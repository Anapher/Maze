using System;

namespace RemoteDesktop.Client.Encoder
{
    public interface IFrameTransmitter
    {
        ArraySegment<byte> AllocateBuffer(int length);
        void ReleaseSendBuffer(ArraySegment<byte> buffer);

        void SendFrame(ArraySegment<byte> sendBuffer);
    }
}