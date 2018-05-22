using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;

namespace Orcus.Server.Service.ModulesV1
{
    public static class ModuleDownloader
    {
        private const int ConfigNugetDownloadBufferSize = 8192;

        private static readonly Lazy<HttpClient> HttpClient =
            new Lazy<HttpClient>(CreateHttpClient, LazyThreadSafetyMode.ExecutionAndPublication);

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            UserAgent.SetUserAgent(client);

            return client;
        }

        public static async Task DownloadModuleAsync(this SourceRepository sourceRepository,
            PackageIdentity packageIdentity, Stream targetStream, ILogger logger, CancellationToken cancellationToken)
        {
            var downloadResource = sourceRepository.GetResource<DownloadResource>();
            var downloadUri = await downloadResource.GetDownloadUrl(packageIdentity, logger, cancellationToken);

            var client = HttpClient.Value;
            var targetStreamPosition = targetStream.Position;

            for (var retry = 0; retry < 3; ++retry)
                using (var response = await client.GetAsync(downloadUri, cancellationToken))
                {
                    if(!response.IsSuccessStatusCode)
                        if (retry == 2)
                            response.EnsureSuccessStatusCode();
                        else
                            continue;

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        if (targetStream.CanSeek)
                            targetStream.Position = targetStreamPosition;

                        try
                        {
                            await stream.CopyToAsync(targetStream);
                        }
                        catch (HttpRequestException)
                        {
                            if (retry == 2) throw;
                        }
                    }
                }
        }

        public static async Task<FileInfo> DirectDownloadAsync(this SourceRepository sourceRepository,
            PackageIdentity packageIdentity, string directory, ILogger logger, CancellationToken cancellationToken)
        {
            var path = Path.Combine(directory, $"{Path.GetRandomFileName()}.nugetdirectdownload");

            var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read,
                ConfigNugetDownloadBufferSize, FileOptions.Asynchronous);

            try
            {
                await sourceRepository.DownloadModuleAsync(packageIdentity, fileStream, logger, cancellationToken);

                var targetFile = new FileInfo(Path.Combine(directory, packageIdentity.GetFilename() + ".orpkg"));
                if (targetFile.Exists) targetFile.Delete();

                File.Move(path, targetFile.FullName);
                targetFile.Refresh();

                return targetFile;
            }
            catch (Exception)
            {
                fileStream.Dispose();
                File.Delete(path);
                throw;
            }
        }
    }
}