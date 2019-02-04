using System;
using System.Net.Http;

namespace Maze.Administration.TestingHelpers.Rest.Comparers
{
    public class RouteComparer : IRequestComparer
    {
        public bool Compare(HttpRequestMessage requestA, HttpRequestMessage requestB)
        {
            var path1 = requestA.RequestUri.GetLeftPart(UriPartial.Path);
            var path2 = requestB.RequestUri.GetLeftPart(UriPartial.Path);

            return path1.Equals(path2);
        }
    }
}