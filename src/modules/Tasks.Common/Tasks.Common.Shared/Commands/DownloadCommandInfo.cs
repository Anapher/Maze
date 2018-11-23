using Force.Crc32;
using System;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Tasks.Infrastructure.Core.Commands;

namespace Tasks.Common.Shared.Commands
{
    public class DownloadCommandInfo : CommandInfo
    {
        [XmlAttribute]
        public string TargetPath { get; set; }

        [XmlAttribute]
        public FileExistsBehavior FileExistsBehavior { get; set; }

        public FileSource FileSource { get; set; }
    }

    public enum FileExistsBehavior
    {
        NoAction,
        ReplaceFile,
        SaveWithDifferentName,
        AttemptToReplaceElseSaveWithDifferentName
    }

    public class FileSource : IXmlSerializable
    {
        public const string Base64Scheme = "base64";

        public FileSource(Uri data, Uri checksum)
        {
            Data = data;
            Checksum = checksum;
        }

        public FileSource()
        {
        }

        public Uri Data { get; set; }
        public Uri Checksum { get; set; }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            Checksum = new Uri(reader.GetAttribute("Checksum"), UriKind.Absolute);
            Data = new Uri(reader.ReadContentAsString(), UriKind.Absolute);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Checksum", Checksum.AbsoluteUri);
            writer.WriteString(Data.AbsoluteUri);
        }
    }

    public enum FileHashAlgorithm
    {
        SHA256,
        SHA512,
        SHA1,
        MD5,
        CRC32
    }

    public static class FileHashAlgorithmExtesnions
    {
        public static HashAlgorithm CreateHashAlgorithm(this FileHashAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case FileHashAlgorithm.SHA256:
                    return SHA256.Create();
                case FileHashAlgorithm.SHA512:
                    return SHA512.Create();
                case FileHashAlgorithm.SHA1:
                    return SHA1.Create();
                case FileHashAlgorithm.MD5:
                    return MD5.Create();
                case FileHashAlgorithm.CRC32:
                    return new Crc32Algorithm();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
