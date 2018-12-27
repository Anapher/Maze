using System;
using System.Threading.Tasks;

namespace Orcus.Sockets.Internal
{
    public delegate Task OnSendPackageDelegate(ArraySegment<byte> data);
}