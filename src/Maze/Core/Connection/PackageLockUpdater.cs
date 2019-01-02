using System;
using System.IO.Abstractions;
using System.Linq;
using Maze.Options;
using Maze.Server.Connection.Modules;
using Maze.Server.Connection.Utilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Maze.Core.Connection
{
    public interface IPackageLockUpdater
    {
        bool Verify(PackagesLock packagesLock);
    }

    public class PackageLockUpdater : IPackageLockUpdater
    {
        private readonly IFileSystem _fileSystem;
        private readonly ModulesOptions _options;

        public PackageLockUpdater(IOptions<ModulesOptions> options, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _options = options.Value;
        }

        public bool Verify(PackagesLock packagesLock)
        {
            var packagesLockFile = _fileSystem.FileInfo.FromFileName(Environment.ExpandEnvironmentVariables(_options.ModulesLockPath));
            if (!packagesLockFile.Exists)
            {
                _fileSystem.File.WriteAllText(packagesLockFile.FullName, JsonConvert.SerializeObject(packagesLock));
                return true;
            }

            var currentPackagesLock = JsonConvert.DeserializeObject<PackagesLock>(_fileSystem.File.ReadAllText(packagesLockFile.FullName));
            var status = CheckPackageLockStatus(currentPackagesLock, packagesLock);
            if (status == PackageLockStatus.Equal)
                return true;

            _fileSystem.File.WriteAllText(packagesLockFile.FullName, JsonConvert.SerializeObject(packagesLock));

            if (status == PackageLockStatus.PackagesAddedOrRemoved)
                return true; //packages can be added

            //if status == PackageLockStatus.Changed
            return false;
        }

        private PackageLockStatus CheckPackageLockStatus(PackagesLock currentLock, PackagesLock newLock)
        {
            foreach (var packageLockEntry in newLock)
            {
                if (currentLock.TryGetValue(packageLockEntry.Key, out var dependencies))
                {
                    if (!packageLockEntry.Value.ScrambledEquals(dependencies))
                        return
                            PackageLockStatus.Changed; //something seems fishy as the dependencies don't match even if the package versions are equal
                }
                else if (currentLock.Any(x => string.Equals(x.Key.Id, packageLockEntry.Key.Id, StringComparison.OrdinalIgnoreCase)))
                {
                    //version changed
                    return PackageLockStatus.Changed;
                }
                else
                {
                    return PackageLockStatus.PackagesAddedOrRemoved;
                }
            }

            if (currentLock.Any(x => !newLock.Any(y => string.Equals(y.Key.Id, x.Key.Id, StringComparison.OrdinalIgnoreCase))))
                return PackageLockStatus.PackagesAddedOrRemoved;

            //we don't really care if packages were removed
            return PackageLockStatus.Equal;
        }

        public enum PackageLockStatus
        {
            /// <summary>
            ///     The package locks are equal
            /// </summary>
            Equal,

            /// <summary>
            ///     Some packages were added or removed
            /// </summary>
            PackagesAddedOrRemoved,

            /// <summary>
            ///     Some package versions changed
            /// </summary>
            Changed
        }
    }
}