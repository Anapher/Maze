using System.Linq;
using System.Net.Http;

namespace Maze.Administration.TestingHelpers.Rest.Comparers
{
    public class HttpBodyComparer : IRequestComparer
    {
        public bool Compare(HttpRequestMessage requestA, HttpRequestMessage requestB)
        {
            var body1 = requestA.Content.ReadAsByteArrayAsync().Result;
            var body2 = requestB.Content.ReadAsByteArrayAsync().Result;

            return body1.SequenceEqual(body2);
        }
    }
}