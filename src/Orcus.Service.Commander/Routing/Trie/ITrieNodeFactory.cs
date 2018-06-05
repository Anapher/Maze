using Orcus.Server.Service.Modules.Routing.Trie.Nodes;

namespace Orcus.Server.Service.Modules.Routing.Trie
{
    /// <summary>
    ///     Factory for creating trie nodes from route definition segments
    /// </summary>
    public interface ITrieNodeFactory
    {
        /// <summary>
        ///     Gets the correct Trie node type for the given segment
        /// </summary>
        /// <param name="parent">Parent node</param>
        /// <param name="segment">Segment</param>
        /// <param name="routeDescription">The route description that defines the segment</param>
        /// <returns>Corresponding TrieNode instance</returns>
        TrieNode GetNodeForSegment(TrieNode parent, string segment, RouteDescription routeDescription);

        TrieNode GetRootNode();
    }
}