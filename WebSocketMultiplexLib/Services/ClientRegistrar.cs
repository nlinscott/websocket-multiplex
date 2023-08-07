using System;
using System.Collections.Generic;

namespace WebSocketMultiplexLib.Services
{
    internal sealed class ClientRegistrar : IClientRegistrar, IDisposable
    {
        private readonly object _lock = new object();

        private readonly IDictionary<ISubscriber, IWebSocketSubscriber> _clients = new Dictionary<ISubscriber, IWebSocketSubscriber>(SubscriberKeyComparer.Instance);

        public void Add(ISubscriber sub, IWebSocketSubscriber socket)
        {
            if (!_clients.ContainsKey(sub))
            {
                lock (_lock)
                {
                    _clients.Add(sub, socket);
                }
            }
        }

        public IWebSocketSubscriber Get(ISubscriber sub)
        {
            IWebSocketSubscriber socket;
            if (_clients.TryGetValue(sub, out socket))
            {
                return socket;
            }

            return null;
        }

        public void Remove(ISubscriber sub)
        {
            IWebSocketSubscriber socket = Get(sub);

            if (socket == null)
            {
                return;
            }

            socket.Dispose();

            lock (_lock)
            {
                _clients.Remove(sub);
            }
        }

        #region IDisposable

        private bool disposedValue;

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    { 
                        foreach (ISubscriber service in _clients.Keys)
                        {
                            Remove(service);
                        }
                    }
                    catch
                    {
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
