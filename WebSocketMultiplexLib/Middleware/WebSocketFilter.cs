using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using System.Threading.Tasks;
using WebSocketMultiplexLib.Model;
using WebSocketMultiplexLib.Services;
using WebSocketMultiplexLib.Services.Channel;

namespace WebSocketMultiplexLib.Middleware
{
    internal sealed class WebSocketFilter
    {
        private readonly ILogger _logger;
        private readonly IChannelSerializer _serializer;
        private readonly IConnectionStateService _connectionService;
        public WebSocketFilter(ILogger<WebSocketFilter> logger, IChannelSerializer serializer, IConnectionStateService connectionService,  RequestDelegate next)
        {
            _logger = logger;
            _serializer = serializer;
            _connectionService = connectionService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            IConnectionInfo info = new ConnectionInfoModel()
            {
                Connection = context.Connection,
                Headers = context.Request.Headers
            };

            if (context.WebSockets == null || !context.WebSockets.IsWebSocketRequest)
            {
                _logger.LogDebug("Recieved request: {Info} but it was not a web socket request. Responding with 400 Bad Request", info);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            _logger.LogDebug("Connected client: {Info}", info);

            WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();

            IWebSocketSubscriber messaging = new WebSocketSubscriber(socket);
            ISubscriber subscriber = new SubscriberModel(info);

            await _serializer.PropagateChannels(messaging);

            await _connectionService.OnConnect(subscriber, messaging);
        }
    }
}
