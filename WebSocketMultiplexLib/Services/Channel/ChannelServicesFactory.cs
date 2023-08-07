using System;
using System.Threading.Tasks;

namespace WebSocketMultiplexLib.Services.Channel
{
    internal sealed class ChannelServicesFactory : IChannelServicesFactory
    {
        private readonly IDisconnectService _disconnectService;
        private readonly IClientRegistrar _clientRegistrar;
        private readonly IChannelMessageConverter _channelMessageConverter;

        public ChannelServicesFactory(IDisconnectService disconnectService, IClientRegistrar clientRegistrar, IChannelMessageConverter channelMessageConverter)
        {
            _disconnectService = disconnectService;
            _clientRegistrar = clientRegistrar;
            _channelMessageConverter = channelMessageConverter;
        }

        public IChannelServices Create(string channelName, ISubscriberCollection subscribers)
        {
            IChannelMessaging messaging = new ChannelMessaging(subscribers, _clientRegistrar, _channelMessageConverter, channelName);

            return new ChannelServicesProxy(messaging, _disconnectService);
        }

        private sealed class ChannelServicesProxy : IChannelServices
        {
            private readonly IChannelMessaging _messaging;
            private readonly IDisconnectService _disconnectService;

            public ChannelServicesProxy(IChannelMessaging messaging, IDisconnectService disconnectService)
            {
                _messaging = messaging;
                _disconnectService = disconnectService;
            }

            public Task BroadcastToAllAsync<T>(T messageObject) where T : class
            {
                return _messaging.BroadcastToAllAsync(messageObject);
            }

            public Task BroadcastRawToAllAsync(ArraySegment<byte> data)
            {
                return _messaging.BroadcastRawToAllAsync(data);
            }

            public Task BroadcastToAllButSenderAsync<T>(T messageObject, ISubscriber sender) where T : class
            {
                return _messaging.BroadcastToAllButSenderAsync(messageObject, sender);
            }

            public Task BroadcastRawToAllButSenderAsync(ArraySegment<byte> data, ISubscriber sender)
            {
                return _messaging.BroadcastRawToAllButSenderAsync(data, sender);
            }

            public void DisconnectSession(ISubscriber sub)
            {
                _disconnectService.DisconnectSession(sub);
            }

            public Task SendToAsync<T>(ISubscriber to, T messageObject) where T : class
            {
                return _messaging.SendToAsync(to, messageObject);
            }

            public Task SendRawToAsync(ISubscriber to, ArraySegment<byte> data)
            {
                return _messaging.SendRawToAsync(to, data);
            }
        }
    }
}
