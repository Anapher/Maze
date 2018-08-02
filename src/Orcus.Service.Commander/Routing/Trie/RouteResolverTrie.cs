using System;
using System.Collections.Generic;
using System.Linq;
using Orcus.Service.Commander.Routing.Trie.Nodes;

namespace Orcus.Service.Commander.Routing.Trie
{
    public class RouteResolverTrie : IRouteResolverTrie
    {
        private static readonly char[] SplitSeparators = {'/'};
        private static readonly MatchResult[] NoResults = new MatchResult[0];
        private readonly ITrieNodeFactory _nodeFactory;
        private readonly IDictionary<string, TrieNode> _routeTries;

        public RouteResolverTrie(ITrieNodeFactory nodeFactory)
        {
            _nodeFactory = nodeFactory;
            _routeTries = new Dictionary<string, TrieNode>(StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public void BuildTrie(IRouteCache cache)
        {
            foreach (var routeDescription in cache.Routes.Select(x => x.Key))
            {
                if (!_routeTries.TryGetValue(routeDescription.Method, out var rootNode))
                {
                    rootNode = _nodeFactory.GetRootNode();
                    _routeTries.Add(routeDescription.Method, rootNode);
                }

                rootNode.Add(routeDescription);
            }
        }

        /// <inheritdoc />
        public MatchResult[] GetMatches(string method, string path)
        {
            if (!_routeTries.TryGetValue(method, out var result))
                return NoResults;

            return result.GetMatches(path.Split(SplitSeparators, StringSplitOptions.RemoveEmptyEntries)).ToArray();
        }
    }
}