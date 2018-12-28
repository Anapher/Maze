using Maze.Administration.Library.Clients.Helpers;

namespace Maze.Administration.Library.Clients
{
    public abstract class ModuleResource<TResource> : Resource<TResource> where TResource : IResourceId, new()
    {
        private readonly string _moduleName;
        private readonly string _resource;

        protected ModuleResource(string moduleName, string resource)
        {
            _moduleName = moduleName;
            _resource = resource;
        }

        public int Version { get; set; } = 1;

        public override string ResourceUri => $"{_moduleName}/v{Version}/{_resource}";
    }
}