using System;
using System.Collections.Generic;

namespace WebSocketMultiplexLib.Services.Channel
{
    internal sealed class ThreadSafeSubscriberCollection : ISubscriberCollection
    {
        private readonly ISet<ISubscriber> _subscribers;

        private readonly object _lock = new object();

        public ThreadSafeSubscriberCollection(ISet<ISubscriber> subscribers)
        {
            _subscribers = subscribers;
        }

        public bool Contains(ISubscriber sub)
        {
            return _subscribers.Contains(sub);
        }

        public void ForEach(Action<ISubscriber> action)
        {
            lock (_lock)
            {
                foreach (ISubscriber sub in _subscribers)
                {
                    action(sub);
                }
            }
        }

        public IEnumerable<TResult> ForEach<TResult>(Func<ISubscriber, TResult> action)
        {
            List<TResult> results = new List<TResult>();
            lock (_lock)
            {
                foreach (ISubscriber sub in _subscribers)
                {
                    TResult result = action(sub);
                    results.Add(result);
                }
            }

            return results;
        }

        public bool TryAdd(ISubscriber sub)
        {
            lock (_lock)
            {
                return _subscribers.Add(sub);
            }
        }

        public bool TryRemove(ISubscriber sub)
        {
            lock (_lock)
            {
                return _subscribers.Remove(sub);
            }
        }
    }
}
