using System.Threading.Tasks;

namespace WebSocketMultiplexLib.Services
{
    interface IConnectionStateService : IDisconnectService
    {
        void OnDisconnect(ISubscriber service);

        Task OnConnect(ISubscriber sub, IWebSocketSubscriber service);
    }
}
