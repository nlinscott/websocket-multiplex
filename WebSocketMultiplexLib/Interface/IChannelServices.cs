using WebSocketMultiplexLib.Services;

namespace WebSocketMultiplexLib
{
    public interface IChannelServices : IDisconnectService, IChannelMessaging
    {
    }
}
