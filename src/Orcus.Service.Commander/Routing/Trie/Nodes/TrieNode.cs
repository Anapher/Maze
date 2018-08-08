using System;
using System.Collections.Generic;
using System.Linq;

namespace Orcus.Service.Commander.Routing.Trie.Nodes
{
    public abstract class TrieNode
    {
        private readonly ITrieNodeFactory _nodeFactory;

        protected TrieNode(TrieNode parent, string segment, ITrieNodeFactory nodeFactory)
        {
            Parent = parent;
            RouteDefinitionSegment = segment;
            _nodeFactory = nodeFactory;

            NodeData = new List<NodeData>();
            Children = new Dictionary<string, TrieNode>(StringComparer.OrdinalIgnoreCase);
        }

        public TrieNode Parent { get; }

        /// <summary>
        ///     Gets or sets the children of this node
        /// </summary>
        public IDictionary<string, TrieNode> Children { get; protected set; }

        /// <summary>
        ///     Gets or sets the segment from the route definition that this node represents
        /// </summary>
        public string RouteDefinitionSegment { get; protected set; }

        /// <summary>
        ///     Score for this node
        /// </summary>
        public abstract int Score { get; }

        /// <summary>
        ///     Gets or sets the node data stored at this node, which will be converted
        ///     into the <see cref="MatchResult" /> if a match is found
        /// </summary>
        public IList<NodeData> NodeData { get; protected set; }

        /// <summary>
        /// Gets all matches for a given requested route
        /// </summary>
        /// <param name="segments">Requested route segments</param>
        /// <param name="context">Current Nancy context</param>
        /// <returns>A collection of <see cref="MatchResult"/> objects</returns>
        public virtual IEnumerable<MatchResult> GetMatches(string[] segments)
        {
            return GetMatches(segments, 0, new Dictionary<string, object>());
        }

        /// <summary>
        /// Gets all matches for a given requested route
        /// </summary>
        /// <param name="segments">Requested route segments</param>
        /// <param name="currentIndex">Current index in the route segments</param>
        /// <param name="capturedParameters">Currently captured parameters</param>
        /// <returns>A collection of <see cref="MatchResult"/> objects</returns>
        public virtual IEnumerable<MatchResult> GetMatches(string[] segments, int currentIndex,
            IDictionary<string, object> capturedParameters)
        {
            var segmentMatch = Match(segments[currentIndex]);
            if (!segmentMatch.Matches)
                return Enumerable.Empty<MatchResult>();

            if (NoMoreSegments(segments, currentIndex))
                return BuildResults(capturedParameters, segmentMatch.CapturedParameters);

            currentIndex++;
            return GetMatchingChildren(segments, currentIndex, capturedParameters, segmentMatch.CapturedParameters);
        }

        /// <summary>
        ///     Matches the segment for a requested route
        /// </summary>
        /// <param name="segment">Segment string</param>
        /// <returns>A <see cref="SegmentMatch" /> instance representing the result of the match</returns>
        public abstract SegmentMatch Match(string segment);

        /// <summary>
        ///     Returns whether we are at the end of the segments
        /// </summary>
        /// <param name="segments">Route segments</param>
        /// <param name="currentIndex">Current index</param>
        /// <returns>True if no more segments left, false otherwise</returns>
        protected bool NoMoreSegments(string[] segments, int currentIndex)
        {
            return currentIndex >= segments.Length - 1;
        }

        protected IEnumerable<MatchResult> BuildResults(IDictionary<string, object> capturedParameters,
            IDictionary<string, object> localCaptures)
        {
            var parameters = new Dictionary<string, object>(capturedParameters);

            if (!NodeData.Any()) //occurres when no route ends here
                return Enumerable.Empty<MatchResult>();

            foreach (var localCapture in localCaptures)
                parameters[localCapture.Key] = localCapture.Value;

            return NodeData.Select(x => MatchResult.FromNodeData(x, parameters));
        }

        /// <summary>
        ///     Add a new route to the trie
        /// </summary>
        /// <param name="routeDescription">The route description</param>
        public void Add(RouteDescription routeDescription)
        {
            var segments = new string[routeDescription.Segments.Length + 1];
            segments[0] = routeDescription.PackageIdentity.Id;
            Array.Copy(routeDescription.Segments, 0, segments, 1, routeDescription.Segments.Length);

            Add(segments, -1, 0, 0, routeDescription);
        }

        /// <summary>
        ///     Add a new route to the trie
        /// </summary>
        /// <param name="segments">The segments of the route definition</param>
        /// <param name="currentIndex">Current index in the segments array</param>
        /// <param name="currentScore">Current score for this route</param>
        /// <param name="nodeCount">Number of nodes added for this route</param>
        /// <param name="routeDescription">The route description</param>
        public virtual void Add(string[] segments, int currentIndex, int currentScore, int nodeCount,
            RouteDescription routeDescription)
        {
            if (NoMoreSegments(segments, currentIndex))
            {
                NodeData.Add(BuildNodeData(nodeCount, currentScore + Score, routeDescription));
                return;
            }

            nodeCount++;
            currentIndex++;

            if (!Children.TryGetValue(segments[currentIndex], out var child))
            {
                child = _nodeFactory.GetNodeForSegment(this, segments[currentIndex], routeDescription);
                Children.Add(segments[currentIndex], child);
            }

            child.Add(segments, currentIndex, currentScore + Score, nodeCount, routeDescription);
        }

        /// <summary>
        ///     Build the node data that will be used to create the <see cref="MatchResult" />
        ///     We calculate/store as much as possible at build time to reduce match time.
        /// </summary>
        /// <param name="nodeCount">Number of nodes in the route</param>
        /// <param name="score">Score for the route</param>
        /// <param name="routeDescription">The route description</param>
        /// <returns>A NodeData instance</returns>
        protected virtual NodeData BuildNodeData(int nodeCount, int score, RouteDescription routeDescription)
        {
            return new NodeData
            {
                Method = routeDescription.Method,
                RouteLength = nodeCount,
                Score = score,
                RouteDescription = routeDescription
            };
        }

        /// <summary>
        ///     Gets all the matches from this node's children
        /// </summary>
        /// <param name="segments">Requested route segments</param>
        /// <param name="currentIndex">Current index</param>
        /// <param name="capturedParameters">Currently captured parameters</param>
        /// <param name="localCaptures">Parameters captured by the local matching</param>
        /// <returns>Collection of <see cref="MatchResult" /> objects</returns>
        protected IEnumerable<MatchResult> GetMatchingChildren(string[] segments, int currentIndex,
            IDictionary<string, object> capturedParameters, IDictionary<string, object> localCaptures)
        {
            var parameters = capturedParameters;
            if (localCaptures.Any())
            {
                parameters = new Dictionary<string, object>(parameters);

                foreach (var localParameter in localCaptures)
                    parameters[localParameter.Key] = localParameter.Value;
            }

            foreach (var childNode in Children.Values)
            foreach (var match in childNode.GetMatches(segments, currentIndex, parameters))
                yield return match;
        }
    }
}