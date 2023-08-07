using System.Collections.Generic;

namespace WebSocketMultiplexLib.Services.Channel
{
    internal interface IChannelCache
    {
        IServerChannel GetChannelBy(string name);

        IEnumerable<string> ChannelNames
        {
            get;
        }

        IEnumerable<IServerChannel> Channels
        {
            get;
        }

        IEnumerable<IServerChannel> GetChannelsSubscribedToBy(ISubscriber sub);
    }
}
