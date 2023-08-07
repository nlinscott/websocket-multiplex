using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebSocketMultiplexLib.Services.Channel
{
    internal sealed class ChannelMessageRouter : IChannelMessageRouter
    {
        private readonly IChannelCache _cache;
        private readonly ILogger _logger;

        public ChannelMessageRouter(IChannelCache cache, ILogger<ChannelMessageRouter> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task ToggleSubscription(ISubscriber subscriber, string channelName)
        {
            IServerChannel channel = _cache.GetChannelBy(channelName);

            if (channel.IsSubscribed(subscriber))
            {
                return channel.Unsubscribe(subscriber);
            }
            
            return channel.Subscribe(subscriber);
        }

        public async Task UnsubscribeFromAll(ISubscriber subscriber)
        {
            IEnumerable<IServerChannel> channels = _cache.GetChannelsSubscribedToBy(subscriber);

            foreach(IServerChannel channel in channels)
            {
               await channel.Unsubscribe(subscriber);
            }
        }

        public Task SendMessageToChannel(ISubscriber sender, string channelName, string message)
        {
            IServerChannel channel = _cache.GetChannelBy(channelName);

            if (channel == null)
            {
                _logger.LogDebug("Sender: {Info} trying to send message to channel {Channel} but channel is not found", sender.ConnectionInfo, channelName);
                return Task.CompletedTask;
            }

            if (!channel.IsSubscribed(sender))
            {
                _logger.LogDebug("Sender: {Info} trying to send message to channel {Channel} but is not subscribed", sender.ConnectionInfo, channelName);

                return Task.CompletedTask;
            }

            return channel.ForwardToChannel(sender, message);
        }
    }
}
