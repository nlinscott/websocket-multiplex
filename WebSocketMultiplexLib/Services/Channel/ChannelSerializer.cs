using System.Text.Json;
using System.Threading.Tasks;

namespace WebSocketMultiplexLib.Services.Channel
{
    internal sealed class ChannelSerializer : IChannelSerializer
    {
        private readonly IChannelCache _cache;

        public ChannelSerializer(IChannelCache cache)
        {
            _cache = cache;
        }

        public Task PropagateChannels(IWebSocketSubscriber service)
        {
            byte[] objData = JsonSerializer.SerializeToUtf8Bytes(_cache.ChannelNames);
            return service.Send(objData);
        }
    }
}
