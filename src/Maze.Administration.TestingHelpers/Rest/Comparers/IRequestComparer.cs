using System.Net.Http;

namespace Maze.Administration.TestingHelpers.Rest.Comparers
{
    public interface IRequestComparer
    {
        bool Compare(HttpRequestMessage requestA, HttpRequestMessage requestB);
    }
}