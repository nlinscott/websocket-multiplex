using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using WebSocketMultiplexLib.Services;
using WebSocketMultiplexLib.Services.Channel;

namespace WebSocketMultiplexLib
{
    internal sealed class ChannelMessaging : IChannelMessaging
    {
        private readonly ISubscriberCollection _subscribers;
        private readonly IClientRegistrar _registrar;
        private readonly byte[] _channelName;

        public ChannelMessaging(ISubscriberCollection subscribers, IClientRegistrar registrar, string channelName)
        {
            _registrar = registrar;
            _subscribers = subscribers;
            _channelName = System.Text.Encoding.UTF8.GetBytes(channelName);
        }

        public Task BroadcastToAllAsync<T>(T messageObject) where T : class
        {
            byte[] message = CreateMessageRaw(messageObject);

            return BroadcastRawToAllAsync(message);
        }

        public Task BroadcastRawToAllAsync(ArraySegment<byte> data)
        {
            data = AppendChannelName(data.Array);
            IEnumerable<Task> sending = _subscribers.ForEach((s) =>
            {
                return SendRawToAsync(s, data);
            });

            return Task.WhenAll(sending);
        }

        public Task BroadcastRawToAllButSenderAsync(ArraySegment<byte> data, ISubscriber sender)
        {
            data = AppendChannelName(data.Array);
            IEnumerable<Task> sending = _subscribers.ForEach((s) =>
            {
                if (s.Equals(sender))
                {
                    return Task.CompletedTask;
                }

                return SendRawToAsync(s, data);
            });

            return Task.WhenAll(sending);
        }

        public Task BroadcastToAllButSenderAsync<T>(T messageObject, ISubscriber sender) where T : class
        {
            byte[] message = CreateMessageRaw(messageObject);

            return BroadcastRawToAllButSenderAsync(message, sender);
        }

        public Task SendToAsync<T>(ISubscriber to, T messageObject) where T : class
        {
            byte[] message = CreateMessageRaw(messageObject);

            return SendRawToAsync(to, message);
        }

        public Task SendRawToAsync(ISubscriber to, ArraySegment<byte> data)
        {
            IWebSocketSubscriber socket = _registrar.Get(to);

            if (socket == null)
            {
                return Task.CompletedTask;
            }

            return socket.Send(data);
        }

        private byte[] CreateMessageRaw<T>(T messageObject)
        {
            byte[] objData = JsonSerializer.SerializeToUtf8Bytes(messageObject);

            return AppendChannelName(objData);
        }

        private byte[] AppendChannelName(byte[] message)
        {
            byte[] buffer = new byte[_channelName.Length + message.Length];
            _channelName.CopyTo(buffer, 0);
            message.CopyTo(buffer, _channelName.Length);

            return buffer;
        }
    }
}
