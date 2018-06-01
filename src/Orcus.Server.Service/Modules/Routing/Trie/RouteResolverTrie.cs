using System;
using System.Collections.Generic;
using System.Linq;
using Orcus.Server.Service.Modules.Routing.Trie.Nodes;

namespace Orcus.Server.Service.Modules.Routing.Trie
{
    public interface IRouteResolverTrie
    {
        /// <summary>
        ///     Build the trie from the route cache
        /// </summary>
        /// <param name="cache">The route cache</param>
        void BuildTrie(IRouteCache cache);

        /// <summary>
        ///     Get all matches for the given method and path
        /// </summary>
        /// <param name="method">HTTP method</param>
        /// <param name="path">Requested path</param>
        /// <returns>An array of <see cref="MatchResult" /> elements</returns>
        MatchResult[] GetMatches(string method, string path);
    }

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

        /// <summary>
        ///     Build the trie from the route cache
        /// </summary>
        /// <param name="cache">The route cache</param>
        public void BuildTrie(IRouteCache cache)
        {
            foreach (var routeDescription in cache.Routes)
            {
                if (!_routeTries.TryGetValue(routeDescription.Method, out var rootNode))
                {
                    rootNode = _nodeFactory.GetRootNode();
                    _routeTries.Add(routeDescription.Method, rootNode);
                }

                rootNode.Add(routeDescription);
            }
        }

        /// <summary>
        ///     Get all matches for the given method and path
        /// </summary>
        /// <param name="method">HTTP method</param>
        /// <param name="path">Requested path</param>
        /// <returns>An array of <see cref="MatchResult" /> elements</returns>
        public MatchResult[] GetMatches(string method, string path)
        {
            if (!_routeTries.TryGetValue(method, out var result))
                return NoResults;

            return result.GetMatches(path.Split(SplitSeparators, StringSplitOptions.RemoveEmptyEntries)).ToArray();
        }
    }
}