#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.IO.Zip
nuget Fake.Tools.Git
nuget Fake.DotNet.MSBuild
nuget Fake.DotNet.Cli
nuget Fake.Api.GitHub
nuget Fake.Core.ReleaseNotes
nuget Fake.Net.Http
nuget Fake.Core.Target //"

#load "./.fake/build.fsx/intellisense.fsx"

open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core
open Fake.Net

open Fake.DotNet
open Fake.Tools.Git.CommandHelper
open Fake.Tools.Git

open System.IO

let buildDir = "./build/"
let artifactsDir = "./artifacts/"
let toolsDir = "./tools/"
let branch = Information.getBranchName "."

let latestGitCommitOfDir dir = runSimpleGitCommand "." <| sprintf """log -n 1 --format="%%h" -- "%s" """ dir
let versionOfChangelog changelogPath = (File.read changelogPath |> Changelog.parse).LatestEntry.SemVer
let getChangelog projectName = "changelogs" </> sprintf "%s.md" projectName
let getVersionPrefix path = System.Text.RegularExpressions.Regex.Match(File.ReadAllText(path), "(?<=<VersionPrefix>).*?(?=<\\/VersionPrefix>)").Value;
let toString o = o.ToString()

let nugetUrl = "https://github.com/MazeAdmin/NuGet.Client/releases/download/5.0.1-rtm/NuGet.exe"
let nugetPath = toolsDir </> "NuGet.5.0.1-rtm.exe"

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

    //projectDir |> DotNet.build (fun opts -> {opts with Configuration = DotNet.BuildConfiguration.Release
    //                                                   NoRestore = true
    //                                                   Common = DotNet.Options.Create().WithCommon(fun options ->
    //                                                       {options with CustomParams = Some "--no-dependencies"}
    //                                                   )
    //                           })

    projectDir |> DotNet.pack (fun opts -> { opts with VersionSuffix = Some suffix
                                                       OutputPath = Some artifact
                                                       Configuration = DotNet.BuildConfiguration.Release
                                                       NoRestore = true
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
    
let executePowerShellScript scriptPath arguments =
    CreateProcess.fromRawCommandLine "powershell.exe" <| sprintf """-NoProfile -ExecutionPolicy Bypass -File "%s" %s""" (Path.getFullName scriptPath) arguments
    |> CreateProcess.withWorkingDirectory (Path.getDirectory scriptPath)
    |> Proc.run // start with the above configuration
    |> (fun x ->
            if x.ExitCode <> 0 then
                failwith <| sprintf "Script exited with code %i" x.ExitCode)

Target.create "Create NuGet Packages" (fun _ ->
    buildNupkgWithChangelog "Administration.ControllerExtensions"
    buildNupkgWithChangelog "Administration.Library"
    buildNupkgWithChangelog "Administration.TestingHelpers"
    buildNupkgWithChangelog "Client.Library"
    buildNupkgWithChangelog "ControllerExtensions"
    buildNupkgWithChangelog "ModuleManagement"
    buildNupkgWithChangelog "Utilities"
    buildNupkgWithChangelog "Modules.Api"
)

Target.create "Build Administration" (fun _ ->
    buildProjectWithChangelog "Administration" ("src" </> "administration.props")
)

Target.create "Build Server" (fun _ ->
    buildProjectWithChangelog "Server" ("src" </> "server.props")
)

Target.create "Prepare Tools" (fun _ ->
    if File.exists nugetPath = false then
        Shell.mkdir toolsDir
        Http.downloadFile nugetPath nugetUrl |> ignore
        Trace.logfn "NuGet.exe downloaded at %s" nugetPath
)

Target.create "Build Modules" (fun _ ->
    let runDotnet options command args =
        let result = DotNet.exec options command args
        if result.ExitCode <> 0 then
            let errors = System.String.Join(System.Environment.NewLine,result.Errors)
            Trace.traceError <| System.String.Join(System.Environment.NewLine,result.Messages)
            failwithf "dotnet process exited with %d: %s" result.ExitCode errors

    let modulesDir = "src" </> "modules"
    let modules = System.IO.Directory.GetDirectories(path=modulesDir)
    let modulesTargetDir = buildDir </> "modules"
    let artifactsTargetDir = artifactsDir </> "modules"
    let packer = "./src/ModulePacker/ModulePacker.csproj"

    Shell.cleanDir modulesTargetDir

    Trace.log "Prebuild all modules"
    modules |> Seq.iter (fun modulePath ->
        if File.exists (modulePath </> "prebuild.ps1") then
            Trace.logfn "Execute prebuild.ps1 in %s" modulePath
            executePowerShellScript (modulePath </> "prebuild.ps1") ""
    )

    modules |> Seq.iter (fun modulePath ->
        let moduleName = Path.GetFileName(modulePath)

        Trace.logfn "Process module %s at %s" moduleName modulePath

        let projectFiles = !! (modulePath </> "**/*.csproj") |> Seq.filter (fun x -> System.Text.RegularExpressions.Regex.IsMatch (Path.GetFileName(x), (sprintf "^%s\.(Administration|Client|Server)\.csproj$" moduleName)))
        let targetDir = modulesTargetDir </> moduleName

        let changelogVersion = versionOfChangelog (modulePath </> "changelog.md") |> toString
        let commitHash = latestGitCommitOfDir modulePath

        let version = changelogVersion + "+" + commitHash

        Trace.logfn "Module %s has version %s" moduleName version
        
        projectFiles |> Seq.iter(fun projectFile ->
            runDotnet (fun o -> o) "pack" <| sprintf """-c Release -o "%s" /p:Version=%s %s""" targetDir version projectFile
        )

        let packageDir = targetDir </> "package"
        runDotnet (fun o -> o) "run" <| sprintf """--project "%s" --framework netcoreapp2.1 -- %s --name %s -o "%s" """ packer targetDir moduleName packageDir

        CreateProcess.fromRawCommandLine nugetPath <| sprintf """pack "%s" -OutputDirectory "%s" """ packageDir artifactsTargetDir
        |> Proc.run // start with the above configuration
        |> (fun x ->
                if x.ExitCode <> 0 then
                    failwith <| sprintf "NuGet exited with code %i" x.ExitCode)

        if File.exists (modulePath </> "postbuild.ps1") then
            Trace.logfn "Execute postbuild.ps1"
            executePowerShellScript (modulePath </> "postbuild.ps1") <| sprintf """-Package "%s" """ (Path.getFullName (artifactsTargetDir </> (sprintf "%s.%s.nupkg" moduleName changelogVersion)))

        Trace.logfn "Module %s created" moduleName
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

    let output = buildDir </> "templates" </> "module"

    MSBuild.runRelease id output "Build" [(projectDir </> "MazeTemplates.Wizard.csproj")]
      |> Trace.logItems "AppBuild-Output: "

    let artifactsOutput = artifactsDir </> "templates" </> "module";
    Shell.mkdir artifactsOutput;
    File.Move(output </> "MazeTemplates.Wizard.vsix", artifactsOutput </> "MazeTemplates.Wizard.vsix");
)

Target.create "Restore Solution" (fun _ ->
    DotNet.restore (fun opts -> opts) "Maze.sln"
)

Target.create "Cleanup" (fun _ ->
    Shell.cleanDir buildDir
    Shell.cleanDir artifactsDir
)

Target.create "Test" (fun _ ->
    let buildMode = Environment.environVarOrDefault "buildMode" "Release"
    MSBuild.build (fun opts -> {opts with Properties =
                                                    [
                                                        "Optimize", "True"
                                                        "DebugSymbols", "True"
                                                        "Configuration", buildMode
                                                    ]
        }) "src\modules\RemoteDesktop\libraries\OpenH264net"
)

Target.create "Compile Native Projects" (fun _ ->
    executePowerShellScript "src\\modules\\RemoteDesktop\\prebuild.ps1" ""

    let buildMode = Environment.environVarOrDefault "buildMode" "Release"
    let compile path = 
        MSBuild.build (fun opts -> {opts with Properties =
                                                        [
                                                            "Optimize", "True"
                                                            "DebugSymbols", "True"
                                                            "Configuration", buildMode
                                                        ]
            }) path

    compile "src\modules\RemoteDesktop\libraries\OpenH264net"
    compile "src\modules\RemoteDesktop\libraries\\x264net"
)

Target.create "All" ignore

// Dependencies
open Fake.Core.TargetOperators

"Cleanup"
  ==> "Restore Solution"
  ==> "Create NuGet Packages"
  ==> "Build Modules"
  ==> "Create VS Template for Module"
  ==> "Build Administration"
  ==> "Build Server"
  ==> "All"

"Cleanup"
  ==> "Prepare Tools"
  ==> "Build Modules"

"Compile Native Projects" ==> "Restore Solution" ==> "Build Modules"

// start build
Target.runOrDefault "All"
