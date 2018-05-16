namespace Orcus.Administration.Core.Clients.Helpers
{
    public abstract class VersionedResource<TResource> : Resource<TResource> where TResource : IResourceId, new()
    {
        private readonly string _resource;

        protected VersionedResource(string resource)
        {
            _resource = resource;
        }

        public int Version { get; set; } = 1;

        public override string ResourceUri => $"v{Version}/{_resource}";
    }
}
