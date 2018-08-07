using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using CommandLine;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace ModulePacker
{
    public class ModulePackerOptions
    {
        [Value(0, Required = true, HelpText = "The directory that contains the nuget packages")]
        public string DirectoryPath { get; set; }

        [Option('o', "output", Required = false, HelpText = "The output directory")]
        public string Output { get; set; }

        [Option("name", Required = false, HelpText = "The name of the module. This application will search for name.[client|server|administration].nupkg in the directory.")]
        public string ModuleName { get; set; }

        [Option("delete", Default = false, HelpText = "Delete the source packages")]
        public bool DeleteSourcePackages { get; set; }
    }

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

            FileInfo[] packages;
            if (!string.IsNullOrEmpty(obj.ModuleName))
            {
                packages = directory.GetFiles($"{obj.ModuleName}.*.nupkg");
            }
            else
            {
                packages = directory.GetFiles("*.nupkg");

                var firstGroup = packages.GroupBy(x => x.Name.Split('.').First()).First();
                obj.ModuleName = firstGroup.Key;
                packages = firstGroup.ToArray();
            }

            if (!packages.Any())
            {
                Console.Error.WriteLine("No packages found");
                return -1;
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

    public static class FrameworkExtensions
    {
        public static NuGetFramework ToNuGetFramework(this OrcusFramework orcusFramework)
        {
            switch (orcusFramework)
            {
                case OrcusFramework.Administration:
                    return FrameworkConstants.CommonFrameworks.OrcusAdministration10;
                case OrcusFramework.Server:
                    return FrameworkConstants.CommonFrameworks.OrcusServer10;
                case OrcusFramework.Client:
                    return FrameworkConstants.CommonFrameworks.OrcusClient10;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orcusFramework), orcusFramework, null);
            }
        }
    }

    public enum OrcusFramework
    {
        Administration,
        Server,
        Client
    }

    public class PseudoNuspecBuilder
    {
        private NuGetVersion _version;
        private readonly Dictionary<string, string> _metadata;

        public PseudoNuspecBuilder()
        {
            _metadata = new Dictionary<string, string>();
            Dependencies = new Dictionary<OrcusFramework, List<PackageDependency>>();
        }

        public Dictionary<OrcusFramework, List<PackageDependency>> Dependencies { get; }

        public void Import(NuspecCoreReader reader, OrcusFramework framework)
        {
            foreach (var metadataKeyValue in reader.GetMetadata())
            {
                switch (metadataKeyValue.Key)
                {
                    case "id":
                        continue;
                    case "version":
                        var version = NuGetVersion.Parse(metadataKeyValue.Value);
                        if (_version != null && _version != version)
                        {
                            Console.Error.WriteLine("The packages have different versions.");
                        }
                        else _version = version;

                        break;
                    case "description":
                        if (metadataKeyValue.Value == "Package Description")
                            continue;

                        ApplyValue(metadataKeyValue);
                        break;
                    default:
                        ApplyValue(metadataKeyValue);
                        break;
                }
            }

            var dependencies = reader.GetDependencies().ToList();
            Dependencies.Add(framework, dependencies);
        }

        public void Write(Stream stream, string id)
        {
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings {Indent = true}))
            {
                writer.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd");
                writer.WriteStartElement("metadata");
                writer.WriteAttributeString("minClientVersion", "2.12");

                writer.WriteElementString("id", id);
                writer.WriteElementString("version", _version.ToString());

                foreach (var metadata in _metadata)
                {
                    writer.WriteElementString(metadata.Key, metadata.Value);
                }

                writer.WriteStartElement("dependencies");

                foreach (var packageDependency in Dependencies)
                {
                    writer.WriteStartElement("group");
                    writer.WriteAttributeString("targetFramework", packageDependency.Key.ToNuGetFramework().ToString());

                    foreach (var dependency in packageDependency.Value)
                    {
                        writer.WriteStartElement("dependency");
                        writer.WriteAttributeString("id", dependency.Id);
                        writer.WriteAttributeString("version", dependency.VersionRange.OriginalString);

                        if (dependency.Include.Any())
                            writer.WriteAttributeString("Include", string.Join(",", dependency.Include));
                        if (dependency.Exclude.Any())
                            writer.WriteAttributeString("Include", string.Join(",", dependency.Exclude));

                        writer.WriteEndElement(); //dependency
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement(); //dependencies

                writer.WriteEndElement(); //metadata
                writer.WriteEndElement(); //package
            }
        }

        private void ApplyValue(KeyValuePair<string, string> pair)
        {
            if (_metadata.TryGetValue(pair.Key, out var currentValue))
            {
                if (currentValue != pair.Value)
                {
                    Console.WriteLine($"Warning: mismatch of metadata in packages for key {pair.Key}: Value1: {currentValue}, Value2: {pair.Value}");
                    if (pair.Value.Length > currentValue.Length)
                        _metadata[pair.Key] = pair.Value;
                }
            }
            else _metadata.Add(pair.Key, pair.Value);
        }
    }
}
