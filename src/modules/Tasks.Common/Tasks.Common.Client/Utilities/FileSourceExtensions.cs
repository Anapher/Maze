using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Maze.Client.Library.Services;
using Maze.Server.Connection;
using Maze.Server.Connection.Utilities;
using System;
using System.Buffers;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Client.Library;

namespace Tasks.Common.Client.Utilities
{
    public static class FileSourceExtensions
    {
        public static async Task<bool> WriteTo(this FileSource fileSource, Stream stream, TaskExecutionContext context, ILogger logger = null)
        {
            if (fileSource.Checksum != null)
            {
                var algo = (FileHashAlgorithm) Enum.Parse(typeof(FileHashAlgorithm), fileSource.Checksum.Scheme, ignoreCase: true);

                using (var hashAlgorithm = algo.CreateHashAlgorithm())
                using (var cryptoStream = new CryptoStream(stream, hashAlgorithm, CryptoStreamMode.Write))
                {
                    if (!await WriteData(cryptoStream, fileSource, context, logger))
                        return false;

                    cryptoStream.FlushFinalBlock();

                    var hash = Hash.Parse(fileSource.Checksum.Host);
                    var computedHash = new Hash(hashAlgorithm.Hash);
                    if (!hash.Equals(computedHash))
                    {
                        logger?.LogError("Hash comparison failed. Hash algorithm: {algo}, supplied hash: {suppliedHash}, computed hash: {computedHash}.", hashAlgorithm.ToString(), hash, computedHash);
                        return false;
                    }
                }
            }
            else
            {
                if (!await WriteData(stream, fileSource, context, logger))
                    return false;
            }

            return true;
        }

        private static async Task<bool> WriteData(Stream targetStream, FileSource fileSource, TaskExecutionContext context, ILogger logger)
        {
            if (fileSource.Data.Scheme == FileSource.Base64Scheme)
            {
                logger?.LogDebug("Write Base64 binary to file");

                context.ReportStatus("Write Base64 to file...");

                var buffer = Convert.FromBase64String(fileSource.Data.AbsolutePath);
                targetStream.Write(buffer, 0, buffer.Length);
                return true;
            }

            if (fileSource.Data.Scheme == "http" || fileSource.Data.Scheme == "https")
            {
                context.ReportStatus("Download headers...");

                var httpClient = context.Services.GetRequiredService<IHttpClientService>().Client;
                using (var response = await httpClient.GetAsync(fileSource.Data, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        logger?.LogError("Requesting {uri} failed with status {status}.", fileSource.Data, response.StatusCode);
                        return false;
                    }

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        var buffer = ArrayPool<byte>.Shared.Rent(8192);
                        try
                        {
                            var contentLength = response.Content.Headers.ContentLength;
                            var totalDataLoaded = 0;

                            while (true)
                            {
                                var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                                if (read == 0)
                                    break;

                                targetStream.Write(buffer, 0, read);
                                totalDataLoaded += read;

                                if (contentLength != null)
                                    context.ReportProgress((double) totalDataLoaded / contentLength);

                                context.ReportStatus($"Downloading... ({DataSizeFormatter.BytesToString(totalDataLoaded)})");
                            }
                        }
                        finally
                        {
                            ArrayPool<byte>.Shared.Return(buffer);
                        }
                    }

                    return true;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(fileSource), fileSource.Data.Scheme, "The scheme is not supported as file source.");
        }
    }
}
