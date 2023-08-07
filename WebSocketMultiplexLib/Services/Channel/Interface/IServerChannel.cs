using System;
using System.Threading.Tasks;

namespace WebSocketMultiplexLib.Services.Channel
{
    internal interface IServerChannel : IEquatable<IServerChannel>
    {
        string Key
        {
            get;
        }

        string Name
        {
            get;
        }

        bool IsSubscribed(ISubscriber sub);

        Task Subscribe(ISubscriber sub);

        Task Unsubscribe(ISubscriber sub);

        Task ForwardToChannel(ISubscriber sender, string message);
    }
}
