using System;
using System.Threading.Tasks;

namespace WebSocketMultiplexLib
{
    public interface IChannelMessaging
    {
        Task SendToAsync<T>(ISubscriber to, T messageObject) where T : class;

        Task SendRawToAsync(ISubscriber to, ArraySegment<byte> data);

        Task BroadcastToAllAsync<T>(T messageObject) where T : class;

        Task BroadcastToAllButSenderAsync<T>(T messageObject, ISubscriber sender) where T : class;

        Task BroadcastRawToAllAsync(ArraySegment<byte> data);

        Task BroadcastRawToAllButSenderAsync(ArraySegment<byte> data, ISubscriber sender);
    }
}
