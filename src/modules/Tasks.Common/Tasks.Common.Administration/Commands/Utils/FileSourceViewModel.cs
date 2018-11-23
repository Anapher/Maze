using Orcus.Server.Connection;
using Orcus.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Administration.PropertyGrid.Attributes;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.Commands.Utils
{
    public class FileSourceViewModel
    {
        private const string _binaryPlaceholder = "<Binary>";
        private string _cachedFile;

        [Path(PathMode = PathMode.File)]
        public string LocalPath { get; set; }
        public string Url { get; set; }

        public FileHashAlgorithm HashAlgorithm { get; set; }
        public string FileHash { get; set; }

        public FileSource Build()
        {
            Uri checksum = null;
            if (!string.IsNullOrWhiteSpace(FileHash))
            {
                var checksumBuilder = new UriBuilder();
                checksumBuilder.Scheme = HashAlgorithm.ToString().ToLower();
                checksumBuilder.Host = Hash.Parse(FileHash).ToString();

                checksum = checksumBuilder.Uri;
            }

            var uriBuilder = new UriBuilder();
            if (!string.IsNullOrEmpty(LocalPath))
            {
                uriBuilder.Scheme = FileSource.Base64Scheme;
                uriBuilder.Path = Convert.ToBase64String(File.ReadAllBytes(LocalPath));
            }
            else if (!string.IsNullOrEmpty(Url))
            {
                return new FileSource(new Uri(Url, UriKind.Absolute), checksum);
            }

            return new FileSource(uriBuilder.Uri, checksum);
        }

        public void Initialize(FileSource model)
        {
            var uri = model.Data;
            switch (uri.Scheme)
            {
                case "binary":
                    LocalPath = _binaryPlaceholder;
                    _cachedFile = uri.AbsolutePath;
                    break;
                case "http":
                case "https":
                    Url = uri.AbsoluteUri;
                    break;
            }
        }

        public ValidationResult ValidateInput()
        {
            if (!string.IsNullOrWhiteSpace(FileHash))
            {
                if (!Hash.TryParse(FileHash, out var hash))
                    return new ValidationResult(Tx.T("TasksCommon:Utilities.FileSource.Errors.InvalidFileHash"));

                using (var hashAlgorithm = HashAlgorithm.CreateHashAlgorithm())
                {
                    if (hash.HashData.Length != hashAlgorithm.HashSize / 8)
                        return new ValidationResult(Tx.T("TasksCommon:Utilities.FileSource.Errors.InvalidHashSize", "algorithm", HashAlgorithm.ToString(), "expected", hashAlgorithm.HashSize.ToString(), "current", (FileHash.Length * 8).ToString()));
                }
            }

            if (string.IsNullOrWhiteSpace(LocalPath) && string.IsNullOrWhiteSpace(Url))
                return new ValidationResult(Tx.T("TasksCommon:Utilities.FileSource.Errors.NoFileSourceSelected"));

            if (!string.IsNullOrWhiteSpace(LocalPath) && !string.IsNullOrWhiteSpace(Url))
                return new ValidationResult(Tx.T("TasksCommon:Utilities.FileSource.Errors.OnlyOneSourceAllowed"));

            if (!string.IsNullOrWhiteSpace(LocalPath))
            {
                if (LocalPath == _binaryPlaceholder)
                {
                    if (_cachedFile != null)
                        return ValidationResult.Success;

                    return new ValidationResult(Tx.T("TasksCommon:Utilities.FileSource.Errors.NoCachedFileAvailable"), nameof(LocalPath).Yield());
                }

                try
                {
                    if (!File.Exists(LocalPath))
                        return new ValidationResult(Tx.T("TasksCommon:Utilities.FileSource.Errors.FileDoesNotExist"), nameof(LocalPath).Yield());
                }
                catch (Exception)
                {
                    return new ValidationResult(Tx.T("TasksCommon:Utilities.FileSource.Errors.InvalidLocalPath"), nameof(LocalPath).Yield());
                }

                return ValidationResult.Success;
            }

            if (!string.IsNullOrWhiteSpace(Url))
            {
                try
                {
                    var uri = new Uri(Url, UriKind.Absolute);
                    if (!string.Equals(uri.Scheme, "http", StringComparison.OrdinalIgnoreCase) && !string.Equals(uri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
                        return new ValidationResult(Tx.T("TasksCommon:Utilities.FileSource.Errors.UrlNotHttp"));

                    return ValidationResult.Success;
                }
                catch (Exception)
                {
                    return new ValidationResult(Tx.T("TasksCommon:Utilities.FileSource.Errors.InvalidUri"));
                }
            }

            throw new InvalidOperationException();
        }
    }
}
