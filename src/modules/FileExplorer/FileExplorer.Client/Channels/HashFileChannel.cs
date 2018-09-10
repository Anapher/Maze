using System;
using System.Security.Cryptography;
using System.Threading;
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
        private readonly CancellationTokenSource _cancellationTokenSource;

        public HashFileChannel()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public event EventHandler<ProgressChangedArgs> ProgressChanged;

        public Task<byte[]> ComputeAsync(string path, FileHashAlgorithm hashAlgorithm)
        {
            try
            {
                using (var fileStream = System.IO.File.OpenRead(path))
                using (var progressingStream = new ProgressingReadStream(fileStream, OnProgressChanged))
                using (var hash = CreateHashAlgorithm(hashAlgorithm))
                using (_cancellationTokenSource.Token.Register(() => fileStream.Dispose()))
                {
                    var result = hash.ComputeHash(progressingStream);
                    return Task.FromResult(result);
                }
            }
            catch (Exception) when (_cancellationTokenSource.IsCancellationRequested)
            {
                return null;
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

        public override void Dispose()
        {
            base.Dispose();
            _cancellationTokenSource.Dispose();
        }
    }
}