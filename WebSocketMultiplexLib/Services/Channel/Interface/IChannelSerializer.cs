using System.Threading.Tasks;

namespace WebSocketMultiplexLib.Services.Channel
{
    interface IChannelSerializer
    {
        Task PropagateChannels(IWebSocketSubscriber service);
    }
}
