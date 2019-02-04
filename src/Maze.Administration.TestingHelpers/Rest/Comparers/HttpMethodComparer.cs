using System.Net.Http;

namespace Maze.Administration.TestingHelpers.Rest.Comparers
{
    public class HttpMethodComparer : IRequestComparer
    {
        public bool Compare(HttpRequestMessage requestA, HttpRequestMessage requestB)
        {
            return requestA.Method.Method.Equals(requestB.Method.Method);
        }
    }
}