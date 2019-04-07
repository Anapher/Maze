#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.IO.Zip
nuget Fake.DotNet.MSBuild
nuget Fake.DotNet.Cli
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.IO
open Fake.IO.Globbing.Operators //enables !! and globbing
open Fake.DotNet
open Fake.Core
open Fake.DotNet

let buildDir = "./build/";

// Default target
Target.create "Build Administration" (fun _ ->
    let projectDir = "src/Maze.Administration"
    let output = Path.combine buildDir "admin"

    projectDir
    |> DotNet.build (fun opts ->
        { opts with
            Configuration = DotNet.BuildConfiguration.Release
            OutputPath = Some output
    })
)

Target.create "Build modules" (fun _ ->
    let output = Path.combine buildDir "modules"
    let moduleProjFiles = !! "src/modules/**/*.csproj"
    let modulePackerDir = Path.combine buildDir "modulePacker"

    DotNet.build (fun opts ->
        { opts with
            Configuration = DotNet.BuildConfiguration.Release
            OutputPath = Some modulePackerDir
        }) "src/ModulePacker"

    //let packModule directory name delete = DotNet.exec  Some Path.combine modulePackerDir "ModulePacker.dll"

    //for projectFile in moduleProjFiles do
    //    let moduleName = projectFile.Substring(output.Length + 1)
    //    DotNet.build (fun opts ->
    //        { opts with
    //            Configuration = DotNet.BuildConfiguration.Release
    //            OutputPath = Some (Path.combine output moduleName)
    //    }) projectFile

    //    DotNet.pack 
)

Target.create "Create VS Template for Module" (fun _ ->
    let baseDir = "src/projectTemplates/module"
    let projectDir = Path.combine baseDir "ModuleTemplateWizard"
    let templateDirectory = Path.combine projectDir "ProjectTemplates"

    Shell.cleanDir templateDirectory

    let createTemplate name =
        let dir = Path.combine baseDir name

        Shell.deleteDir (Path.combine dir "bin")
        Shell.deleteDir (Path.combine dir "obj")

        !! (Path.combine dir "**/*")
        |> Zip.zip dir (Path.combine templateDirectory name + ".zip")

    createTemplate "ModuleTemplate.Client"
    createTemplate "ModuleTemplate.Administration"

    let output = Path.combine buildDir "templates/module"
    !! (Path.combine projectDir "MazeTemplates.Wizard.csproj")
      |> MSBuild.runRelease id output "Build"
      |> Trace.logItems "AppBuild-Output: "
)

Target.create "Build Server" (fun _ ->
    let projectDir = "src/Maze.Server"
    let output = Path.combine buildDir "server"

    projectDir
    |> DotNet.build (fun opts ->
        { opts with
            Configuration = DotNet.BuildConfiguration.Release
            OutputPath = Some output
    })
)

Target.create "Cleanup" (fun _ ->
    Shell.cleanDir buildDir
)

// Dependencies
open Fake.Core.TargetOperators

"Cleanup"
  ==> "Build Administration"
  ==> "Create VS Template for Module"
  ==> "Build Server"

// start build
Target.runOrDefault "Create VS Template for Module"