using System;
using System.Threading.Tasks;

namespace WebSocketMultiplexLib.Services
{
    internal interface IListenerService : IDisposable
    {
        Task ListenUntilDisconnect(ISubscriber sub, IWebSocketSubscriber messaging, Action onDisconnect);
    }
}
