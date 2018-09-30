using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;
using NuGet.Packaging.Core;

namespace ModulePacker
{
    class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<ModulePackerOptions>(args).MapResult(ExecuteAction, errors => -1);
        }

        private static int ExecuteAction(ModulePackerOptions obj)
        {
            var directory = new DirectoryInfo(obj.DirectoryPath);
            if (!directory.Exists)
            {
                Console.Error.WriteLine($"The directory '{directory.FullName}' was not found");
                return -1;
            }

            List<FileInfo> packages;
            if (!string.IsNullOrEmpty(obj.ModuleName))
            {
                packages = directory.GetFiles($"{obj.ModuleName}.*.nupkg").ToList();
            }
            else
            {
                packages = directory.GetFiles("*.nupkg").ToList();

                var firstGroup = packages.GroupBy(x => x.Name.Split('.').First()).First();
                obj.ModuleName = firstGroup.Key;
                packages = firstGroup.ToList();
            }

            if (!packages.Any())
            {
                Console.Error.WriteLine("No packages found for {0}", obj.ModuleName);
                return -1;
            }

            var actualPackage = packages.FirstOrDefault(x => Regex.Match(x.Name, $"^{obj.ModuleName}\\.[0-9]").Success);
            if (actualPackage != null)
            {
                Console.WriteLine("Package is already created at {0}", actualPackage.Name);
                if (packages.Count == 1)
                    return -1;

                packages.Remove(actualPackage);
            }

            var output = obj.Output;
            if (string.IsNullOrEmpty(output))
                output = Path.Combine(obj.DirectoryPath, obj.ModuleName);

            var outputDirectory = new DirectoryInfo(output);
            if (outputDirectory.Exists)
                outputDirectory.Delete(true);

            outputDirectory.Create();
            Console.WriteLine($"Packages that will be merged in directory {output}");
            foreach (var fileInfo in packages)
                Console.WriteLine($"\t{fileInfo.Name}");

            var builder = new PseudoNuspecBuilder();
            foreach (var fileInfo in packages)
            {
                CopyNupkgToOutput(outputDirectory, obj.ModuleName, fileInfo.FullName, builder);
            }

            using (var outputStream = File.Create(Path.Combine(outputDirectory.FullName, $"{obj.ModuleName}.nuspec")))
                builder.Write(outputStream, obj.ModuleName);

            if (obj.DeleteSourcePackages)
            {
                Console.WriteLine("Cleanup, delete source packages...");
                foreach (var fileInfo in packages)
                    fileInfo.Delete();
            }

            return 0;
        }

        private static void CopyNupkgToOutput(DirectoryInfo directory, string moduleName, string packagePath, PseudoNuspecBuilder builder)
        {
            using (var sourceFile = ZipFile.OpenRead(packagePath))
            {
                var nuspecFile = sourceFile.Entries.Single(x => x.Name.EndsWith(".nuspec"));
                TryParseFrameworkFromName(nuspecFile.Name, moduleName, out var framework);

                using (var stream = nuspecFile.Open())
                {
                    var reader = new NuspecCoreReader(stream);

                    var id = reader.GetId();
                    if (framework == null)
                    {
                        if (Enum.TryParse<OrcusFramework>(id.Split('.').Last(), out var fw))
                            framework = fw;
                    }

                    builder.Import(reader, framework.Value);
                }
                
                var nuGetFramework = framework.Value.ToNuGetFramework();

                var targetDirectory = new DirectoryInfo(Path.Combine(directory.FullName, "lib", nuGetFramework.GetShortFolderName()));
                targetDirectory.Create();

                foreach (var zipArchiveEntry in sourceFile.Entries.Where(x => x.FullName.StartsWith("lib/")))
                {
                    using (var targetFileStream =
                        File.Create(Path.Combine(targetDirectory.FullName, zipArchiveEntry.Name)))
                    using (var sourceStream = zipArchiveEntry.Open())
                        sourceStream.CopyTo(targetFileStream);
                }
            }
        }

        private static bool TryParseFrameworkFromName(string name, string moduleName, out OrcusFramework? orcusFramework)
        {
            var match = Regex.Match(name, $"{moduleName}.(?<framework>(.*?))\\.nuspec");
            if (match.Success)
            {
#if NETFRAMEWORK
                orcusFramework =
                    (OrcusFramework) Enum.Parse(typeof(OrcusFramework), match.Groups["framework"].Value, true);
#else
                orcusFramework = Enum.Parse<OrcusFramework>(match.Groups["framework"].Value, true);
#endif

                return true;
            }

            orcusFramework = null;
            return false;
        }
    }
}
