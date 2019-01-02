extern alias extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Maze.Options;
using Maze.Server.Connection.Clients;
using Maze.Server.Connection.Modules;
using Maze.Server.Connection.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NuGet.Packaging.Core;
using IChangeToken = extensions::Microsoft.Extensions.Primitives.IChangeToken;

namespace Maze.Core.Configuration
{
    public interface IConfigurationManager
    {
        bool IsSynchronized(List<int> configurationHashes);
        void Synchronize(List<ClientConfigurationDataDto> configurations, PackagesLock packagesLock);
    }

    public class ConfigurationManager : IConfigurationManager
    {
        private readonly IFileSystem _fileSystem;
        private readonly ModulesOptions _options;

        public ConfigurationManager(IOptions<ModulesOptions> options, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _options = options.Value;
        }

        public bool IsSynchronized(List<int> configurationHashes)
        {
            var existingConfigurations = new List<int>();

            var configurationDirectory = _fileSystem.DirectoryInfo.FromDirectoryName(Environment.ExpandEnvironmentVariables(Program.ConfigDirectory));
            foreach (var fileInfo in configurationDirectory.GetFiles("mazesettings*.json"))
                existingConfigurations.Add((int) MurmurHash2.Hash(File.ReadAllText(fileInfo.FullName)));

            return configurationHashes.ScrambledEquals(existingConfigurations);
        }

        public void Synchronize(List<ClientConfigurationDataDto> configurations, PackagesLock packagesLock)
        {
            var configurationDirectory = _fileSystem.DirectoryInfo.FromDirectoryName(Environment.ExpandEnvironmentVariables(Program.ConfigDirectory));
            var existingConfigurations = configurationDirectory.GetFiles("mazesettings*.json")
                .Select(x => Regex.Match(x.Name, "mazesettings([0-9])\\.json")).Select(x => x.Success ? int.Parse(x.Groups[0].Value) : (int?) null);

            foreach (var configurationId in existingConfigurations.Where(x => configurations.All(y => y.ClientGroupId != x)))
                _fileSystem.File.Delete(Path.Combine(Program.ConfigDirectory, $"mazesettings{configurationId}.json"));

//            var filesProvider = new MemoryFileProvider(configurations.ToDictionary(x => $"mazesettings{x.ClientGroupId}.json", x => x.Content));
//            var configBuilder = new ConfigurationBuilder();
//#if DEBUG
//            configBuilder.AddJsonFile("mazesettings.json");
//#endif
//            foreach (var configuration in configurations)
//            {
//                configBuilder.AddJsonFile(filesProvider, $"mazesettings{configuration.ClientGroupId}.json", optional: false, reloadOnChange: false);
//            }

            //var configurationRoot = configBuilder.Build();
            ////var modules = configurationRoot.GetSection("LoadOnStartup").GetChildren();
            ////var packagesConfig = packagesLock; //TrimPackagesLock(modules.Select(x => x.Key), packagesLock);

            //_fileSystem.File.WriteAllText(Environment.ExpandEnvironmentVariables(_options.ModulesLockPath),
            //    JsonConvert.SerializeObject(packagesConfig));

            foreach (var configuration in configurations)
            {
                var filename = Path.Combine(Environment.ExpandEnvironmentVariables(Program.ConfigDirectory), $"mazesettings{configuration.ClientGroupId}.json");
                _fileSystem.File.WriteAllText(filename, configuration.Content);
            }
        }

        //private PackagesLock TrimPackagesLock(IEnumerable<string> modules, PackagesLock packagesLock)
        //{
        //    var resultPackagesLock = new Dictionary<PackageIdentity, IImmutableList<PackageIdentity>>();

        //    foreach (var module in modules)
        //    {
        //        var packageIdentity = packagesLock.First(x => string.Equals(x.Key.Id, module, StringComparison.OrdinalIgnoreCase));
        //        AddModule(packageIdentity.Key, resultPackagesLock, packagesLock);
        //    }

        //    return new PackagesLock(resultPackagesLock);
        //}

        //private void AddModule(PackageIdentity packageIdentity, 
        //    Dictionary<PackageIdentity, IImmutableList<PackageIdentity>> packagesDictionary, PackagesLock sourceLock)
        //{
        //    if (!packagesDictionary.ContainsKey(packageIdentity))
        //        packagesDictionary.Add(packageIdentity, sourceLock[packageIdentity]);

        //    var dependencies = sourceLock[packageIdentity];
        //    foreach (var dependency in dependencies)
        //    {
        //        AddModule(dependency, packagesDictionary, sourceLock);
        //    }
        //}
    }

    public class MemoryFileProvider : IFileProvider
    {
        private readonly IDictionary<string, byte[]> _files;

        public MemoryFileProvider(IDictionary<string, string> files)
        {
            _files = files.ToDictionary(x => x.Key, x => Encoding.UTF8.GetBytes(x.Value));
        }

        public MemoryFileProvider(IDictionary<string, byte[]> files)
        {
            _files = files;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (_files.TryGetValue(subpath, out var value))
                return new VirtualFileInfo(subpath, value);

            return new NotFoundFileInfo(subpath);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            throw new NotSupportedException();
        }

        public IChangeToken Watch(string filter)
        {
            throw new NotSupportedException();
        }
    }

    public class VirtualFileInfo : IFileInfo
    {
        private readonly byte[] _content;

        public VirtualFileInfo(string name, byte[] content)
        {
            Name = name;
            _content = content;
        }

        public Stream CreateReadStream() => new MemoryStream(_content, false);

        public bool Exists { get; } = true;
        public long Length => _content.Length;
        public string PhysicalPath => Name;
        public string Name { get; }
        public DateTimeOffset LastModified => DateTimeOffset.UtcNow;
        public bool IsDirectory { get; } = false;
    }

}