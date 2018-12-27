namespace Orcus.Service.Commander.Routing.Trie.Nodes
{
    public class CaptureNode : TrieNode
    {
        private readonly string _parameterName;

        public CaptureNode(TrieNode parent, string segment, ITrieNodeFactory nodeFactory) : base(parent, segment,
            nodeFactory)
        {
            _parameterName = RouteDefinitionSegment.Trim('{', '}');
        }

        public override int Score { get; } = 1000;

        public override SegmentMatch Match(string segment)
        {
            var match = new SegmentMatch(true);
            match.CapturedParameters.Add(_parameterName, segment);
            return match;
        }
    }
}