using System.Collections.Generic;

namespace Maze.Service.Commander.Routing.Trie.Nodes
{
    public class RootNode : TrieNode
    {
        private readonly SegmentMatch _successMatch = new SegmentMatch(true);

        public RootNode(ITrieNodeFactory trieNodeFactory) : base(null, null,
            trieNodeFactory)
        {
        }
        
        public override int Score { get; } = 0;

        public override IEnumerable<MatchResult> GetMatches(string[] segments, int currentIndex, IDictionary<string, object> capturedParameters)
        {
            var localCaptures = new Dictionary<string, object>();

            if (segments.Length == 0)
                return BuildResults(capturedParameters, localCaptures);

            return GetMatchingChildren(segments, currentIndex, capturedParameters, localCaptures);
        }

        public override SegmentMatch Match(string segment)
        {
            return _successMatch;
        }
    }
}