using System.Threading.Tasks;
using WebSocketMultiplexLib.Services.Channel;

namespace WebSocketMultiplexLib.Services
{
    internal sealed class ConnectionStateService : IConnectionStateService
    {
        private readonly IClientRegistrar _registrar;
        private readonly IListenerService _listenerService;
        private readonly IChannelMessageRouter _messageRouter;

        public ConnectionStateService(IClientRegistrar registrar, IListenerService listenerService, IChannelMessageRouter messageRouter)
        {
            _registrar = registrar;
            _listenerService = listenerService;
            _messageRouter = messageRouter;
        }

        public Task OnConnect(ISubscriber sub, IWebSocketSubscriber socket)
        {
            _registrar.Add(sub, socket);

            return _listenerService.ListenUntilDisconnect(sub, socket, () =>
            {
                OnDisconnect(sub);
            });
        }

        public void OnDisconnect(ISubscriber sub)
        {
            _messageRouter.UnsubscribeFromAll(sub);

            _registrar.Remove(sub);
        }

        public void DisconnectSession(ISubscriber sub)
        {
            IWebSocketSubscriber socket = _registrar.Get(sub);

            if(socket == null)
            {
                return;
            }

            OnDisconnect(sub);
        }
    }
}
