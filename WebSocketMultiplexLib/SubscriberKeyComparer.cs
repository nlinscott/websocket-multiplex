using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WebSocketMultiplexLib
{
    public sealed class SubscriberKeyComparer : IEqualityComparer<ISubscriber>
    {
        private static readonly SubscriberKeyComparer _instance = new SubscriberKeyComparer();
        public static IEqualityComparer<ISubscriber> Instance
        {
            get
            {
                return _instance;
            }
        }

        private SubscriberKeyComparer() { }

        public bool Equals(ISubscriber x, ISubscriber y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return string.Equals(x.SessionId, y.SessionId, System.StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode([DisallowNull] ISubscriber obj)
        {
            return obj.SessionId.GetHashCode();
        }
    }
}
