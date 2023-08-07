using System;
using System.Collections.Generic;

namespace WebSocketMultiplexLib.Services.Channel
{
    internal interface ISubscriberCollection
    {
        bool TryAdd(ISubscriber sub);

        bool TryRemove(ISubscriber sub);

        bool Contains(ISubscriber sub);

        void ForEach(Action<ISubscriber> action);

        IEnumerable<TResult> ForEach<TResult>(Func<ISubscriber, TResult> action);
    }
}
