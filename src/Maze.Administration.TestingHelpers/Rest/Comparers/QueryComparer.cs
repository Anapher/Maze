using System.Net.Http;

namespace Maze.Administration.TestingHelpers.Rest.Comparers
{
    public class QueryComparer : IRequestComparer
    {
        public bool Compare(HttpRequestMessage requestA, HttpRequestMessage requestB)
        {
            return requestA.RequestUri.Query.Equals(requestB.RequestUri.Query);
        }
    }
}