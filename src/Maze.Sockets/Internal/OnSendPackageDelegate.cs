using System;
using System.Threading.Tasks;

namespace Maze.Sockets.Internal
{
    public delegate Task OnSendPackageDelegate(ArraySegment<byte> data);
}