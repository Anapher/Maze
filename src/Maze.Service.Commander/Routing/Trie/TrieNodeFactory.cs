using Maze.Service.Commander.Routing.Trie.Nodes;

namespace Maze.Service.Commander.Routing.Trie
{
    public class TrieNodeFactory : ITrieNodeFactory
    {
        public TrieNode GetNodeForSegment(TrieNode parent, string segment, RouteDescription routeDescription)
        {
            var chars = segment.ToCharArray();
            var start = chars[0];
            var end = chars[chars.Length - 1];

            if (start == '{' && end == '}')
                return new CaptureNode(parent, segment, this);

            return new LiteralNode(parent, segment, this);
        }

        public TrieNode GetRootNode()
        {
            return new RootNode(this);
        }
    }
}