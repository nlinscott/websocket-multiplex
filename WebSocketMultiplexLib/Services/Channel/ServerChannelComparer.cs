using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WebSocketMultiplexLib.Services.Channel
{
    internal sealed class ServerChannelComparer : IEqualityComparer<IServerChannel>
    {
        private static readonly ServerChannelComparer _instance = new ServerChannelComparer();

        public static ServerChannelComparer Instance
        {
            get
            {
                return _instance;
            }
        }

        public bool Equals(IServerChannel x, IServerChannel y)
        {
            if(x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }

            return x.Equals(y);
        }

        public int GetHashCode([DisallowNull] IServerChannel obj)
        {
            return obj.GetHashCode();
        }
    }
}
