using System.IO;
using System.Threading.Tasks;

namespace Orcus.Server.Service.ModulesV2.Extensions
{
    public static class ModuleOptionsExtensions
    {
        public static Task Load(this ModulesOptions options)
        {
            var tasks = new[] {options.ModulesConfig.Reload(), options.ModulesLock.Reload()};

            Directory.CreateDirectory(options.ModulesDirectory);
            Directory.CreateDirectory(options.CacheDirectory);
            Directory.CreateDirectory(options.ConfigDirectory);

            return Task.WhenAll(tasks);
        }
    }
}