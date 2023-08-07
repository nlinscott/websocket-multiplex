using ChatApp.Shared.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;

namespace ChatApp
{
    internal class ChatClient : IDisposable
    {
        private UserModel _user;

        private readonly ClientWebSocket _client = new ClientWebSocket();

        private JArray _channels;

        public async Task Connect(string url, UserModel user)
        {
            _user = user;

            await _client.ConnectAsync(new Uri(url), CancellationToken.None);

            byte[] buffer = new byte[1024];

            WebSocketReceiveResult receiveResult = await _client.ReceiveAsync(buffer, CancellationToken.None);

            _channels = JArray.Parse(Encoding.UTF8.GetString(buffer, 0, receiveResult.Count));

            string loginChannel = _channels[0].ToString();
            string chatChannel = _channels[1].ToString();

            await Login(loginChannel);
            await SubscribeToChannel(chatChannel);
        }

        public Task ListenForMessages(CancellationTokenSource token, Action<string> onMessageRecieved)
        {
            return Task.Run(async () =>
            {
                byte[] buffer = new byte[1024];

                while (!token.IsCancellationRequested)
                {
                    WebSocketReceiveResult receiveResult = await _client.ReceiveAsync(buffer, token.Token);

                    if(receiveResult.MessageType == WebSocketMessageType.Close || token.IsCancellationRequested)
                    {
                        break;
                    }

                    string msg = System.Text.Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);

                    onMessageRecieved(msg);
                }
            });
        }

        public Task SendMessage(string message)
        {
            string chatChannel = _channels[1].ToString();

            string msg = $"{chatChannel}[{_user.Name}]:{message}";

            return _client.SendAsync(Encoding.UTF8.GetBytes(msg), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private Task SubscribeToChannel(string channel)
        {
            return _client.SendAsync(Encoding.UTF8.GetBytes(channel), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task Login(string channel)
        {
            await SubscribeToChannel(channel);

            string userModel = JsonConvert.SerializeObject(_user);

            string loginMsg = channel + userModel;

            await _client.SendAsync(Encoding.UTF8.GetBytes(loginMsg), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        #region IDisposable
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    }
                    catch
                    {
                    }

                    try
                    {
                        _client.Dispose();
                    }
                    catch { }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ChatClient()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
