using System;

namespace WebSocketMultiplexLib.Model
{
    internal sealed class SubscriberModel : ISubscriber
    {
        public string SessionId
        {
            get;
        }

        public IConnectionInfo ConnectionInfo
        {
            get;
        }

        public SubscriberModel(IConnectionInfo info)
        {
            ConnectionInfo = info;
            SessionId = Guid.NewGuid().ToString();
        }
    }
}
