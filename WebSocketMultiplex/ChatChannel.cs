using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WebSocketMultiplexLib;

namespace WebSocketMultiplex
{
    internal sealed class ChatChannel : IChannel
    {
        private readonly ILogger _logger;

        private IChannelServices _services;

        public ChatChannel(ILogger<ChatChannel> logger)
        {
            _logger = logger;
        }

        public void OnChannelReady(IChannelServices services)
        {
            _services = services;
        }

        public Task BeforeChannelShutdown()
        {
            return Task.CompletedTask;
        }

        public Task OnChannelMessage(ISubscriber sender, string message)
        {
            ArraySegment<byte> data = System.Text.Encoding.UTF8.GetBytes(message);

            _services.BroadcastRawToAllButSenderAsync(data, sender);
            return Task.CompletedTask;
        }

        public Task OnSubscribe(ISubscriber subscriber)
        {
            return Task.CompletedTask;
        }

        public Task OnUnsubscribe(ISubscriber subscriber)
        {
            return Task.CompletedTask;
        }
    }
}
