using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Maze.Client.Administration.Core.Wix
{
    public class WixComponentGenerator
    {
        public WixComponentGenerator(string componentGroupName, string directoryReference)
        {
            ComponentGroupName = componentGroupName;
            DirectoryReference = directoryReference;
        }

        public string ComponentGroupName { get; }
        public string DirectoryReference { get; }

        public void Write(IEnumerable<WixFile> wixFiles, XmlWriter writer)
        {
            writer.WriteStartElement("Wix", "http://schemas.microsoft.com/wix/2006/wi");

            var referenceIds = new List<string>();

            writer.WriteStartElement("Fragment");
            writer.WriteStartElement("DirectoryRef");
            writer.WriteAttributeString("Id", DirectoryReference);

            void AddFolder(WixDirectory directory)
            {
                foreach (var file in directory.Files)
                {
                    writer.WriteStartElement("Component");

                    var componentId = "cmp" + Guid.NewGuid().ToString("N");
                    writer.WriteAttributeString("Id", componentId);
                    writer.WriteAttributeString("Guid", Guid.NewGuid().ToString("B").ToUpper());

                    writer.WriteStartElement("File");
                    writer.WriteAttributeString("Id", "fi" + Guid.NewGuid().ToString("N"));
                    writer.WriteAttributeString("KeyPath", "yes");
                    writer.WriteAttributeString("Source", file.Path);
                    writer.WriteEndElement(); //File

                    writer.WriteEndElement(); //Component

                    referenceIds.Add(componentId);
                }

                foreach (var subDirectory in directory.Directories)
                {
                    writer.WriteStartElement("Directory");
                    writer.WriteAttributeString("Id", "dir" + Guid.NewGuid().ToString("N"));
                    writer.WriteAttributeString("Name", subDirectory.Name);

                    AddFolder(subDirectory);

                    writer.WriteEndElement(); //Directory
                }
            }

            var rootDirectory = BuildHierarchy(wixFiles);
            AddFolder(rootDirectory);

            writer.WriteEndElement(); //DirectoryRef
            writer.WriteEndElement(); //Fragment

            writer.WriteStartElement("Fragment");
            writer.WriteStartElement("ComponentGroup");
            writer.WriteAttributeString("Id", ComponentGroupName);

            foreach (var referenceId in referenceIds)
            {
                writer.WriteStartElement("ComponentRef");
                writer.WriteAttributeString("Id", referenceId);
                writer.WriteEndElement(); //ComponentRef
            }

            writer.WriteEndElement(); //Fragment

            writer.WriteEndElement(); //Wix
        }

        private static WixDirectory BuildHierarchy(IEnumerable<WixFile> wixFiles)
        {
            var rootFolder = new WixDirectory(string.Empty);
            var folders = new Dictionary<string, WixDirectory>(StringComparer.OrdinalIgnoreCase) {{rootFolder.Name, rootFolder}};

            foreach (var wixFile in wixFiles)
                if (string.IsNullOrEmpty(wixFile.RelativeFolder))
                {
                    rootFolder.Files.Add(wixFile);
                }
                else
                {
                    if (!folders.TryGetValue(wixFile.RelativeFolder, out var directory))
                    {
                        var segments = wixFile.RelativeFolder.Split(new[] {Path.DirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries);

                        var lastDirectory = rootFolder;
                        for (var i = 0; i < segments.Length; i++)
                        {
                            var path = string.Join(Path.DirectorySeparatorChar.ToString(), segments.Take(i + 1));
                            if (!folders.TryGetValue(path, out directory))
                            {
                                directory = new WixDirectory(segments[i]);
                                folders.Add(path, directory);
                                lastDirectory.Directories.Add(directory);
                            }

                            lastDirectory = directory;
                        }

                        lastDirectory.Files.Add(wixFile);
                    }
                }

            return rootFolder;
        }

        private class WixDirectory
        {
            public WixDirectory(string name)
            {
                Name = name;
            }

            public string Name { get; }
            public List<WixFile> Files { get; } = new List<WixFile>();
            public List<WixDirectory> Directories { get; } = new List<WixDirectory>();
        }
    }

    public class WixFile
    {
        public WixFile(string path, string relativeFolder)
        {
            Path = path;
            RelativeFolder = relativeFolder;
        }

        public string Path { get; set; }
        public string RelativeFolder { get; set; }
    }
}