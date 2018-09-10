using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FileExplorer.Client.Utilities;
using FileExplorer.Shared.Channels;
using Force.Crc32;
using Orcus.ControllerExtensions;
using Orcus.Modules.Api.Routing;

namespace FileExplorer.Client.Channels
{
    [Route("computeHash")]
    public class HashFileChannel : CallTransmissionChannel<IHashFileAction>, IHashFileAction
    {
        public event EventHandler<ProgressChangedArgs> ProgressChanged;

        public Task<byte[]> ComputeAsync(string path, FileHashAlgorithm hashAlgorithm)
        {
            using (var fileStream = System.IO.File.OpenRead(path))
            using (var progressingStream = new ProgressingReadStream(fileStream, OnProgressChanged))
            using (var hash = CreateHashAlgorithm(hashAlgorithm))
            {
                var result = hash.ComputeHash(progressingStream);
                return Task.FromResult(result);
            }
        }

        private void OnProgressChanged(double progress)
        {
            ProgressChanged?.Invoke(this, new ProgressChangedArgs(progress));
        }

        private HashAlgorithm CreateHashAlgorithm(FileHashAlgorithm hashAlgorithm)
        {
            switch (hashAlgorithm)
            {
                case FileHashAlgorithm.MD5:
                    return MD5.Create();
                case FileHashAlgorithm.SHA1:
                    return SHA1.Create();
                case FileHashAlgorithm.SHA256:
                    return SHA256.Create();
                case FileHashAlgorithm.SHA384:
                    return SHA384.Create();
                case FileHashAlgorithm.SHA512:
                    return SHA512.Create();
                case FileHashAlgorithm.CRC32:
                    return new Crc32Algorithm();
                default:
                    throw new ArgumentOutOfRangeException(nameof(hashAlgorithm), hashAlgorithm, null);
            }
        }
    }
}