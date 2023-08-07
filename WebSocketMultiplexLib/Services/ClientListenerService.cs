using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketMultiplexLib.Services
{
    internal sealed class ClientListenerService : IListenerService
    {
        private readonly IMessageParser _parser;

        private readonly ILogger _logger;

        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public ClientListenerService(IMessageParser parser, ILogger<ClientListenerService> logger)
        {
            _parser = parser;
            _logger = logger;
        }

        public Task ListenUntilDisconnect(ISubscriber sub, IWebSocketSubscriber socket, Action onDisconnect)
        {
            return Task.Run(async () =>
            {
                while (!_tokenSource.IsCancellationRequested)
                {
                    string data = await socket.Recieve();

                    if (string.IsNullOrEmpty(data))
                    {
                        _logger.LogDebug("Connection {ConnectionInfo} has been closed", sub.ConnectionInfo);
                        onDisconnect();
                        return;
                    }

                    await _parser.ParseAndRoute(sub, data);
                }
            });
        }

        #region IDisposable

        private bool disposedValue;
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _tokenSource.Cancel();
                    _tokenSource.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }

        #endregion
    }
}
