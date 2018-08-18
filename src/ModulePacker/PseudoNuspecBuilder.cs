using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace ModulePacker
{
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