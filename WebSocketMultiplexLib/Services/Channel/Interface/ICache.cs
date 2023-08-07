using System.Collections.Generic;

namespace WebSocketMultiplexLib.Services.Channel
{
    internal interface ICache<T>
    {
        void SetItems(IReadOnlySet<T> items);
    }
}
