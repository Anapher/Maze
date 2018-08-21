using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;

namespace ModuleLocalUpdater
{
    public class ModuleLocalUpdaterOptions
    {
        [Value(0, Required = true, HelpText = "The name of the module")]
        public string ModuleName { get; set; }

        [Value(1, Required = true, HelpText = "The directory of the compiled module files")]
        public string ModuleDirectory { get; set; }

        [Option("solutionDir", HelpText = "The directory of the solution.", Required = false)]
        public string SolutionDirectory { get; set; }
    }

    internal class Program
    {
        private static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<ModuleLocalUpdaterOptions>(args)
                .MapResult(ExecuteAction, errors => -1);
        }

        private static string GetSolutionDirectory(string path) =>
            path.Substring(0, path.IndexOf("\\src\\", StringComparison.OrdinalIgnoreCase));

        private static DirectoryInfo GetLatestDirectory(IEnumerable<DirectoryInfo> directories)
        {
            return directories.OrderByDescending(x => x.LastWriteTimeUtc).FirstOrDefault();
        }

        private static int ExecuteAction(ModuleLocalUpdaterOptions arg)
        {
            //F:\Projects\Orcus\src\modules\UserInteraction\UserInteraction.Administration\bin\Debug\net47
            //UserInteraction.Administration

            var solutionDirectory = arg.SolutionDirectory ?? GetSolutionDirectory(arg.ModuleDirectory);
            DirectoryInfo packageDirectory;

            var framework = arg.ModuleName.Split('.').Last();
            var moduleName = arg.ModuleName.Substring(0, arg.ModuleName.LastIndexOf('.'));
            string nugetFolderName;

            switch (framework)
            {
                case "Administration":
                case "Client":
                    string packagesDirectory;
                    if (framework == "Administration")
                    {
                        nugetFolderName = "admin10";
                        packagesDirectory =
                            Path.Combine(solutionDirectory, "src\\Orcus.Administration\\bin\\Debug\\packages");
                    }
                    else
                    {
                        nugetFolderName = "client10";
                        packagesDirectory =
                            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                "Orcus\\modules");
                    }

                    packageDirectory =
                        GetLatestDirectory(new DirectoryInfo(packagesDirectory).GetDirectories(moduleName + "*"));
                    break;
                case "Server":
                    packageDirectory =
                        GetLatestDirectory(new DirectoryInfo(Path.Combine(solutionDirectory,
                            "src\\Orcus.Server\\modules\\" + moduleName)).GetDirectories());
                    nugetFolderName = "server10";
                    break;
                default:
                    Console.WriteLine($"Error: Framework '{framework}' could not be identified.");
                    return -1;
            }

            if (packageDirectory?.Exists != true)
            {
                Console.WriteLine($"Target directory '{packageDirectory?.FullName}' does not exist");
                return 0;
            }

            CopyFiles(arg.ModuleDirectory, Path.Combine(packageDirectory.FullName, "lib", nugetFolderName));

            Console.WriteLine("Modules updated successfully");
            return 0;
        }

        private static void CopyFiles(string sourceFolder, string targetFolder)
        {
            var targetFiles = new DirectoryInfo(targetFolder).GetFiles();
            foreach (var fileInfo in targetFiles)
            {
                var sourceFile = new FileInfo(Path.Combine(sourceFolder, fileInfo.Name));
                if (sourceFile.Exists)
                {
                    Console.WriteLine($"{sourceFile.Name} -> {fileInfo.FullName}");
                    sourceFile.CopyTo(fileInfo.FullName, true);
                }
            }

            foreach (var dllFile in targetFiles.Where(x => x.Extension == ".dll"))
            {
                var pdbName = Path.GetFileNameWithoutExtension(dllFile.Name) + ".pdb";
                if (targetFiles.Any(x => x.Name == pdbName))
                    continue;

                var sourcePdbFile = new FileInfo(Path.Combine(sourceFolder, pdbName));
                if (sourcePdbFile.Exists)
                {
                    var targetFile = Path.Combine(targetFolder, pdbName);

                    Console.WriteLine($"{sourcePdbFile.Name} -> {targetFile}");
                    sourcePdbFile.CopyTo(targetFile, true);
                }
            }
        }
    }
}