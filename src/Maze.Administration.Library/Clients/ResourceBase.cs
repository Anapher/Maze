using Maze.Administration.Library.Clients.Helpers;

namespace Maze.Administration.Library.Clients
{
    public abstract class ResourceBase<TResource> : Resource<TResource> where TResource : IResourceId, new()
    {
        protected ResourceBase(string resource)
        {
            ResourceUri = resource;
        }

        public override string ResourceUri { get; }
    }
}