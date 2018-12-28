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

            foreach (var includedReference in builder.IncludedReferences)
            {
                foreach (var dependency in builder.Dependencies)
                {
                    var referenceDependency =
                        dependency.Value.FirstOrDefault(x => x.Id.Equals(includedReference.Id, StringComparison.OrdinalIgnoreCase));
                    if (referenceDependency != null)
                    {
                        dependency.Value.Remove(referenceDependency);

                        foreach (var includedReferenceDependency in includedReference.Dependencies)
                        {
                            if (dependency.Value.Any(x => x.Id.Equals(includedReferenceDependency.Id, StringComparison.OrdinalIgnoreCase)))
                                continue;

                            dependency.Value.Add(includedReferenceDependency);
                        }

                        var targetFolder = Path.Combine(outputDirectory.FullName, "lib", dependency.Key.ToNuGetFramework().GetShortFolderName());
                        foreach (var fileName in Directory.EnumerateFiles(includedReference.ContentPath))
                        {
                            var relativePath = fileName.Remove(0, includedReference.ContentPath.Length + 1);

                            var targetFileInfo = new FileInfo(Path.Combine(targetFolder, relativePath));
                            targetFileInfo.Directory.Create();

                            File.Copy(fileName, targetFileInfo.FullName);
                        }
                    }
                }

                Directory.Delete(includedReference.ContentPath, true);
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
                        if (Enum.TryParse<MazeFramework>(id.Split('.').Last(), out var fw))
                            framework = fw;
                        else
                        {
                            Console.WriteLine($"Package {id} could not be associated to an Maze framework. It will be included as a reference.");

                            var tempFolder = Path.Combine(directory.FullName, reader.GetId());
                            ExtractPackage(tempFolder, sourceFile);

                            builder.IncludedReferences.Add(new IncludedReference
                            {
                                Dependencies = reader.GetDependencies().ToList(), ContentPath = tempFolder, Id = reader.GetId()
                            });
                            return;
                        }
                    }

                    builder.Import(reader, framework.Value);
                }
                
                var nuGetFramework = framework.Value.ToNuGetFramework();
                ExtractPackage(Path.Combine(directory.FullName, "lib", nuGetFramework.GetShortFolderName()), sourceFile);
            }
        }

        private static void ExtractPackage(string targetDirectoryName, ZipArchive archive)
        {
            var targetDirectory = new DirectoryInfo(targetDirectoryName);
            targetDirectory.Create();

            foreach (var zipArchiveEntry in archive.Entries.Where(x => x.FullName.StartsWith("lib/")))
            {
                using (var targetFileStream =
                    File.Create(Path.Combine(targetDirectory.FullName, zipArchiveEntry.Name)))
                using (var sourceStream = zipArchiveEntry.Open())
                    sourceStream.CopyTo(targetFileStream);
            }
        }

        private static bool TryParseFrameworkFromName(string name, string moduleName, out MazeFramework? mazeFramework)
        {
            var match = Regex.Match(name, $"{moduleName}.(?<framework>(.*?))\\.nuspec");
            if (match.Success)
            {
                if (Enum.TryParse(match.Groups["framework"].Value, true, out MazeFramework result))
                {
                    mazeFramework = result;
                    return true;
                }
            }

            mazeFramework = null;
            return false;
        }
    }
}
