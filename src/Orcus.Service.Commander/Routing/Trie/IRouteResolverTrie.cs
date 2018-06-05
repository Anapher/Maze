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
}