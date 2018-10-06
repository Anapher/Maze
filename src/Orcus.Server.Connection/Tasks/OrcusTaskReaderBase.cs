using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Orcus.Server.Connection.Tasks
{
    //Inspired by https://github.com/NuGet/NuGet.Client/blob/dev/src/NuGet.Core/NuGet.Packaging.Core/NuspecCoreReaderBase.cs
    /// <summary>
    ///     A very basic Orcus Task reader that understands the Id, Version, PackageType, and MinClientVersion of a package.
    /// </summary>
    public abstract class OrcusTaskReaderBase
    {
        private XElement _metadataNode;
        private Dictionary<string, string> _metadataValues;

        /// <summary>
        ///     Read a task from a path.
        /// </summary>
        protected OrcusTaskReaderBase(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            Xml = LoadXml(File.OpenRead(path), false);
        }

        /// <summary>
        ///     Read a task from a stream.
        /// </summary>
        protected OrcusTaskReaderBase(Stream stream) : this(stream, false)
        {
        }

        /// <summary>
        ///     Read a task from a stream.
        /// </summary>
        protected OrcusTaskReaderBase(Stream stream, bool leaveStreamOpen)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            Xml = LoadXml(stream, leaveStreamOpen);
        }

        /// <summary>
        ///     Reads a task from XML
        /// </summary>
        protected OrcusTaskReaderBase(XDocument xml)
        {
            Xml = xml ?? throw new ArgumentNullException(nameof(xml));
        }

        /// <summary>
        ///     Indexed metadata values of the XML elements.
        ///     If duplicate keys exist only the first is used.
        /// </summary>
        protected Dictionary<string, string> MetadataValues
        {
            get
            {
                if (_metadataValues == null)
                {
                    var metadataValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    foreach (var pair in GetMetadata())
                        if (!metadataValues.ContainsKey(pair.Key))
                            metadataValues.Add(pair.Key, pair.Value);

                    _metadataValues = metadataValues;
                }

                return _metadataValues;
            }
        }

        protected XElement MetadataNode
        {
            get
            {
                if (_metadataNode == null)
                {
                    // find the metadata node regardless of the NS
                    _metadataNode = Xml.Root.Elements().FirstOrDefault(e => StringComparer.Ordinal.Equals(e.Name.LocalName, XmlNames.Metadata));

                    if (_metadataNode == null)
                        throw new TaskParsingException("The metadata note is missing for the task.");
                }

                return _metadataNode;
            }
        }

        /// <summary>
        ///     Raw XML doc
        /// </summary>
        public XDocument Xml { get; }

        /// <summary>
        ///     Name of the task
        /// </summary>
        public virtual string GetName()
        {
            var node = MetadataNode.Elements(XName.Get(XmlNames.Name, MetadataNode.GetDefaultNamespace().NamespaceName)).FirstOrDefault();
            return node?.Value;
        }

        /// <summary>
        ///     Id of the task
        /// </summary>
        public virtual Guid GetId()
        {
            var node = MetadataNode.Elements(XName.Get(XmlNames.Id, MetadataNode.GetDefaultNamespace().NamespaceName)).FirstOrDefault();
            return node == null ? Guid.Empty : Guid.Parse(node.Value);
        }

        /// <summary>
        ///     Task Metadata
        /// </summary>
        public virtual IEnumerable<KeyValuePair<string, string>> GetMetadata()
        {
            return MetadataNode.Elements().Where(e => !e.HasElements && !string.IsNullOrEmpty(e.Value))
                .Select(e => new KeyValuePair<string, string>(e.Name.LocalName, e.Value));
        }

        /// <summary>
        ///     Returns a task metadata value or string.Empty.
        /// </summary>
        public virtual string GetMetadataValue(string name)
        {
            MetadataValues.TryGetValue(name, out var metadataValue);
            return metadataValue ?? string.Empty;
        }

        private static XDocument LoadXml(Stream stream, bool leaveStreamOpen)
        {
            using (var xmlReader = XmlReader.Create(stream,
                new XmlReaderSettings
                {
                    CloseInput = !leaveStreamOpen, IgnoreWhitespace = true, IgnoreComments = true, IgnoreProcessingInstructions = true
                }))
            {
                return XDocument.Load(xmlReader, LoadOptions.None);
            }
        }
    }
}