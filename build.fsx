#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.IO.Zip
nuget Fake.Tools.Git
nuget Fake.DotNet.MSBuild
nuget Fake.DotNet.Cli
nuget Fake.Api.GitHub
nuget Fake.BuildServer.AppVeyor
nuget Fake.Core.ReleaseNotes
nuget Fake.Core.Target //"

#load "./.fake/build.fsx/intellisense.fsx"

open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core

open Fake.DotNet
open Fake.BuildServer
open Fake.Tools.Git.CommandHelper
open Fake.Tools.Git

open System.IO

BuildServer.install [
    AppVeyor.Installer
]

let buildDir = "./build/"
let artifactsDir = "./artifacts/"
let branch = Information.getBranchName "."


let latestGitCommitOfDir dir = runSimpleGitCommand "." <| sprintf """log -n 1 --format="%%h" -- "%s" """ dir
let versionOfChangelog changelogPath = (File.read changelogPath |> Changelog.parse).LatestEntry.SemVer
let getChangelog projectName = "changelogs" </> sprintf "%s.md" projectName
let getVersionPrefix path = System.Text.RegularExpressions.Regex.Match(File.ReadAllText(path), "(?<=<VersionPrefix>).*?(?=<\\/VersionPrefix>)").Value;
let toString o = o.ToString()

let buildNupkgWithChangelog name =
    let projectDir = "src" </> sprintf "Maze.%s" name
    let artifact = artifactsDir </> "nupkgs"

    let changelogVersion = versionOfChangelog <| getChangelog name
    let projectVersion = getVersionPrefix (projectDir </> (sprintf "Maze.%s.csproj" name)) |> SemVer.parse

    if projectVersion > changelogVersion then
        let projectVersionString = projectVersion.ToString()
        let changelogVersionString = changelogVersion.ToString()
        failwithf "%s has the version %s defined in project, but the changelog has the version %s" name projectVersionString changelogVersionString

    let commit = latestGitCommitOfDir projectDir
    let suffix = sprintf "%s+%s" branch commit

    projectDir |> DotNet.build (fun opts -> {opts with Configuration = DotNet.BuildConfiguration.Release
                                                       NoRestore = true
                                                       Common = DotNet.Options.Create().WithCommon(fun options ->
                                                           {options with CustomParams = Some "--no-dependencies"}
                                                       )
                               })

    projectDir |> DotNet.pack (fun opts -> { opts with VersionSuffix = Some suffix
                                                       OutputPath = Some artifact
                                                       NoBuild = true
                                           })

let buildProjectWithChangelog name versionFile =
    let projectDir = "src" </> sprintf "Maze.%s" name
    let output = buildDir </> name

    let changelogVersion = versionOfChangelog <| getChangelog name
    let projectVersion = getVersionPrefix versionFile |> SemVer.parse

    if projectVersion > changelogVersion then
        let projectVersionString = projectVersion.ToString()
        let changelogVersionString = changelogVersion.ToString()
        failwithf "%s has the version %s defined in project, but the changelog has the version %s" name projectVersionString changelogVersionString

    let commit = latestGitCommitOfDir projectDir
    let suffix = sprintf "%s+%s" branch commit

    projectDir |> DotNet.publish (fun opts -> {opts with VersionSuffix = Some suffix
                                                         NoRestore = true
                                                         OutputPath = Some output
                               })

    let artifactPath = artifactsDir </> (sprintf "Maze.%s.%s-%s.zip" name (projectVersion |> toString) suffix)
    !! (output + "/**/*")
        |> Zip.zip output artifactPath

    Trace.publish ImportData.BuildArtifact artifactPath
    

Target.create "Create NuGet Packages" (fun _ ->
    buildNupkgWithChangelog "Administration.ControllerExtensions"
    buildNupkgWithChangelog "Administration.Library"
    buildNupkgWithChangelog "Administration.TestingHelpers"
    buildNupkgWithChangelog "Client.Library"
    buildNupkgWithChangelog "ControllerExtensions"
    buildNupkgWithChangelog "ModuleManagement"
    buildNupkgWithChangelog "Utilities"
    buildNupkgWithChangelog "Modules.Api"

    let nupkgs = !! (artifactsDir </> "nupkgs")
    nupkgs |> Seq.iter(fun artifactFile ->
        Trace.publish ImportData.BuildArtifact artifactFile
    )
)

Target.create "Build Administration" (fun _ ->
    buildProjectWithChangelog "Administration" ("src" </> "administration.props")
)

Target.create "Build Server" (fun _ ->
    buildProjectWithChangelog "Server" ("src" </> "server.props")
)

Target.create "Build modules" (fun _ ->
    let runDotnet options command args =
        let result = DotNet.exec options command args
        if result.ExitCode <> 0 then
            let errors = System.String.Join(System.Environment.NewLine,result.Errors)
            Trace.traceError <| System.String.Join(System.Environment.NewLine,result.Messages)
            failwithf "dotnet process exited with %d: %s" result.ExitCode errors

    let modulesDir = "./src/modules"
    let modules = System.IO.Directory.GetDirectories(path=modulesDir)
    let modulesTargetDir = "./build/modules"
    let packer = "./src/Module.Packer"

    modules |> Seq.iter (fun modulePath ->
        let moduleName = Path.GetFileName(modulePath)
        let projectFiles = !! (modulePath </> "**/*.csproj") |> Seq.filter (fun x -> System.Text.RegularExpressions.Regex.IsMatch (x, (sprintf "^%s\\.(Administration|Client|Server)" moduleName)))
        let targetDir = modulesTargetDir </> moduleName

        let changelogVersion = versionOfChangelog (modulePath </> "changelog.md") |> toString
        let commitHash = latestGitCommitOfDir targetDir
        let version = changelogVersion + "+" + commitHash
        
        projectFiles |> Seq.iter(fun projectFile ->
            runDotnet (fun o -> {o with WorkingDirectory = Path.getDirectory projectFile }) "pack" <| sprintf """-c Release -o "%s" /p:Version=%s""" targetDir version
        )

        runDotnet (fun o -> {o with WorkingDirectory = packer}) "run" <| sprintf "-- %s --delete" targetDir

        let nupkgName = sprintf "%s.nupkg" moduleName

        File.Move((targetDir </> nupkgName), (modulesDir </> nupkgName))
        Directory.delete targetDir
    )
)

Target.create "Create VS Template for Module" (fun _ ->
    let baseDir = "src" </> "projectTemplates" </> "module"
    let projectDir = baseDir </> "ModuleTemplateWizard"
    let templateDirectory = projectDir </> "ProjectTemplates"

    Shell.cleanDir templateDirectory

    let createTemplate name =
        let dir = baseDir </> name

        Shell.deleteDir (dir </> "bin")
        Shell.deleteDir (dir </> "obj")

        !! (dir </> "**/*") |> Zip.zip dir (templateDirectory </> name + ".zip")

    createTemplate "ModuleTemplate.Client"
    createTemplate "ModuleTemplate.Administration"

    let output = buildDir </> "templates/module"

    MSBuild.runRelease id output "Build" [(projectDir </> "MazeTemplates.Wizard.csproj")]
      |> Trace.logItems "AppBuild-Output: "
)

Target.create "Restore Solution" (fun _ ->
    DotNet.restore (fun opts -> opts) "Maze.sln"
)

Target.create "Cleanup" (fun _ ->
    Shell.cleanDir buildDir
    Shell.cleanDir artifactsDir
)

Target.create "All" ignore

// Dependencies
open Fake.Core.TargetOperators

"Cleanup"
  ==> "Restore Solution"
  ==> "Create NuGet Packages"
  ==> "Build Administration"
  ==> "Build Server"
  ==> "All"

// start build
Target.runOrDefault "All"
