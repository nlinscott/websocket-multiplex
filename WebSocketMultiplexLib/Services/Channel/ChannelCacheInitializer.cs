using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace WebSocketMultiplexLib.Services.Channel
{
    internal sealed class ChannelCacheInitializer
    {
        private readonly ICache<IServerChannel> _cache;
        private readonly IChannelNamePool _namePool;
        private readonly IChannelServicesFactory _factory;

        public ChannelCacheInitializer(ICache<IServerChannel> cache, IChannelNamePool namePool, IChannelServicesFactory factory)
        {
            _cache = cache;
            _namePool = namePool;
            _factory = factory;
        }

        public void PopulateChannelCache(IServiceProvider serviceProvider, IEnumerable<IChannel> channels)
        {
            HashSet<IServerChannel> serverChannels = serviceProvider.GetRequiredService<HashSet<IServerChannel>>();

            foreach (IChannel channel in channels)
            {
                ISubscriberCollection subscribers = serviceProvider.GetRequiredService<ISubscriberCollection>();

                IServerChannel serverChannel = new ServerChannel(channel, subscribers, _namePool);

                serverChannels.Add(serverChannel);

                IChannelServices services = _factory.Create(serverChannel.Name, subscribers);

                channel.OnChannelReady(services);
            }

            _cache.SetItems(serverChannels);
        }
    }
}
