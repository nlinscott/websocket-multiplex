using System;
using System.Collections.Generic;
using System.Linq;

namespace WebSocketMultiplexLib.Services.Channel
{
    internal sealed class ChannelCache : IChannelCache, ICache<IServerChannel>
    {
        private IReadOnlyDictionary<string, IServerChannel> _channelsByName;
        private IReadOnlySet<IServerChannel> _channels;

        public IEnumerable<string> ChannelNames
        {
            get
            {
                return _channelsByName.Keys;
            }
        }

        public IEnumerable<IServerChannel> Channels
        {
            get
            {
                return _channels;
            }
        }

        public IServerChannel GetChannelBy(string name)
        {
            IServerChannel channel;

            if(_channelsByName.TryGetValue(name, out channel))
            {
                return channel;
            }

            return null;
        }

        public IEnumerable<IServerChannel> GetChannelsSubscribedToBy(ISubscriber sub)
        {
            return _channels.Where(s => s.IsSubscribed(sub));
        }

        public void SetItems(IReadOnlySet<IServerChannel> items)
        {
            _channels = items;

            Dictionary<string, IServerChannel> channelsByName = new Dictionary<string, IServerChannel>(StringComparer.InvariantCultureIgnoreCase);

            foreach(IServerChannel channel in _channels)
            {
                channelsByName.Add(channel.Name, channel);
            }

            _channelsByName = channelsByName;
        }
    }
}
