using System;

namespace Maze.Service.Commander.Routing.Trie.Nodes
{
    public class LiteralNode : TrieNode
    {
        public LiteralNode(TrieNode parent, string segment, ITrieNodeFactory nodeFactory) : base(parent, segment,
            nodeFactory)
        {
        }

        public override int Score { get; } = 10000;

        public override SegmentMatch Match(string segment)
        {
            if (string.Equals(segment, RouteDefinitionSegment, StringComparison.OrdinalIgnoreCase))
                return new SegmentMatch(true);

            return SegmentMatch.NoMatch;
        }
    }
}