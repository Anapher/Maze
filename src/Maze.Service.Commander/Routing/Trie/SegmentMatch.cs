using System.Collections.Generic;

namespace Orcus.Service.Commander.Routing.Trie
{
    /// <summary>
    ///     A segment match result
    /// </summary>
    public class SegmentMatch
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SegmentMatch" /> class, with
        ///     the provided <paramref name="matches" />.
        /// </summary>
        /// <param name="matches"><see langword="true" /> if match was successful.</param>
        public SegmentMatch(bool matches)
        {
            Matches = matches;

            if (matches) CapturedParameters = new Dictionary<string, object>();
        }

        /// <summary>
        ///     Gets a value indicating whether the match was successful or not
        /// </summary>
        public bool Matches { get; }

        /// <summary>
        ///     Gets a <see cref="SegmentMatch" /> representing "no match"
        /// </summary>
        public static SegmentMatch NoMatch { get; } = new SegmentMatch(false);

        /// <summary>
        ///     Gets the captured parameters from the match, if the match was successful
        /// </summary>
        public IDictionary<string, object> CapturedParameters { get; }
    }
}