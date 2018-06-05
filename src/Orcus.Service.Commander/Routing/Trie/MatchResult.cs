using System.Collections.Generic;

namespace Orcus.Server.Service.Modules.Routing.Trie
{
    public class MatchResult : NodeData
    {
        public MatchResult(IDictionary<string, object> parameters)
        {
            Parameters = parameters;
        }

        /// <summary>
        ///     Gets or sets the captured parameters
        /// </summary>
        public IDictionary<string, object> Parameters { get; }

        /// <summary>
        ///     Create a match result from node data and parameters
        /// </summary>
        /// <param name="data">The node data that represents the route of the match</param>
        /// <param name="parameters">The parameters captured</param>
        /// <returns>Return the match result</returns>
        public static MatchResult FromNodeData(NodeData data, IDictionary<string, object> parameters)
        {
            return new MatchResult(parameters)
            {
                RouteDescription = data.RouteDescription,
                Method = data.Method,
                RouteLength = data.RouteLength,
                Score = data.Score
            };
        }

        /// <summary>
        ///     Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has the following
        ///     meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This
        ///     object is equal to <paramref name="other" />. Greater than zero This object is greater than
        ///     <paramref name="other" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(MatchResult other)
        {
            // Length of the route takes precedence over score
            if (RouteLength < other.RouteLength)
                return -1;

            if (RouteLength > other.RouteLength)
                return 1;

            if (Score < other.Score)
                return -1;

            if (Score > other.Score)
                return 1;

            return 0;
        }
    }
}