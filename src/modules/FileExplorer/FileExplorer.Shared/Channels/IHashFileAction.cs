using System;
using System.Threading.Tasks;

namespace FileExplorer.Shared.Channels
{
    public interface IHashFileAction
    {
        event EventHandler<ProgressChangedArgs> ProgressChanged;

        Task<byte[]> ComputeAsync(string path, FileHashAlgorithm hashAlgorithm);
    }

    public class ProgressChangedArgs : EventArgs
    {
        public ProgressChangedArgs(double progress)
        {
            Progress = progress;
        }

        public ProgressChangedArgs()
        {
        }

        public double Progress { get; set; }
    }

    public enum FileHashAlgorithm
    {
        MD5,
        SHA1,
        SHA256,
        SHA384,
        SHA512,
        CRC32
    }
}