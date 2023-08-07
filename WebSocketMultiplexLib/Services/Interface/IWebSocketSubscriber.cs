using System;
using System.Threading.Tasks;

namespace WebSocketMultiplexLib.Services
{
    internal interface IWebSocketSubscriber : IDisposable
    {
        Task<string> Recieve();

        Task Send(ArraySegment<byte> data);
    }
}
