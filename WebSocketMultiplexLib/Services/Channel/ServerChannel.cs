using System;
using System.Threading.Tasks;

namespace WebSocketMultiplexLib.Services.Channel
{
    internal sealed class ServerChannel : IServerChannel
    {
        private readonly IChannel _channel;

        private readonly ISubscriberCollection _subscribers;

        public ServerChannel(IChannel channel, ISubscriberCollection subscribers, IChannelNamePool namePool)
        {
            Key = Guid.NewGuid().ToString();
            _channel = channel;
            Name = namePool.Next();

            _subscribers = subscribers;
        }

        public string Key
        {
            get;
        }

        public string Name
        {
            get;
        }

        #region equality
        public bool Equals(IServerChannel other)
        {
            return string.Equals(Key, other.Key, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {

            if(obj is not IServerChannel)
            {
                return false;
            }

            return Equals(obj as IServerChannel);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        #endregion

        public bool IsSubscribed(ISubscriber sub)
        {
            return _subscribers.Contains(sub);
        }

        public Task Subscribe(ISubscriber sub)
        {
            if(_subscribers.TryAdd(sub))
            {
                return _channel.OnSubscribe(sub);
            }

            return Task.CompletedTask;
        }

        public Task Unsubscribe(ISubscriber sub)
        {
            if (_subscribers.TryRemove(sub))
            {
                return _channel.OnUnsubscribe(sub);
            }

            return Task.CompletedTask;
        }

        public Task ForwardToChannel(ISubscriber sender, string message)
        {
            return _channel.OnChannelMessage(sender, message);
        }
    }
}
