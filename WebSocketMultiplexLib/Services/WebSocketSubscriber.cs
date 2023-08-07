using System;
using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketMultiplexLib.Services
{
    internal sealed class WebSocketSubscriber : IWebSocketSubscriber
    {
        private readonly WebSocket _webSocket;

        private const int MAX_BUFFER_SIZE = 512;

        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public WebSocketSubscriber(WebSocket webSocket)
        {
            _webSocket = webSocket;
        }

        public Task Send(ArraySegment<byte> data)
        {
            return _webSocket.SendAsync(data, WebSocketMessageType.Text, true, _tokenSource.Token);
        }

        public async Task<string> Recieve()
        {
            using (IMemoryOwner<byte> buffer = MemoryPool<byte>.Shared.Rent(MAX_BUFFER_SIZE))
            {
                try
                {
                    ValueWebSocketReceiveResult result = await _webSocket.ReceiveAsync(buffer.Memory, _tokenSource.Token);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        return null;
                    }

                    Memory<byte> recieved = buffer.Memory.Slice(0, result.Count);

                    return Encoding.UTF8.GetString(recieved.Span);

                }
                catch
                {
                    return null;
                }
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
                        _tokenSource.Cancel();

                        _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, System.Threading.CancellationToken.None).Wait();
                        _webSocket.Dispose();

                        _tokenSource.Dispose();
                    }
                    catch { }
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
