using ChatApp.Shared.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using WebSocketMultiplex.Services;
using WebSocketMultiplexLib;

namespace WebSocketMultiplex
{
    internal class LoginChannel : IChannel
    {
        private readonly ILogger _logger;
        private readonly ILoginService _loginService;

        private IChannelServices _channelServices;

        public LoginChannel(ILogger<LoginChannel> logger, ILoginService loginService)
        {
            _logger = logger;
            _loginService = loginService;
        }

        public void OnChannelReady(IChannelServices services)
        {
            _channelServices = services;
        }

        public Task BeforeChannelShutdown()
        {
            return Task.CompletedTask;
        }

        public Task OnChannelMessage(ISubscriber sender, string message)
        {
            UserModel model = JsonConvert.DeserializeObject<UserModel>(message);
            _loginService.OnLogin(sender, model);

            string userLoginnotification = $"[SERVER] User {model.Name} has joined the chat.";

            ArraySegment<byte> data = System.Text.Encoding.UTF8.GetBytes(userLoginnotification);

            return _channelServices.BroadcastRawToAllButSenderAsync(data, sender);
        }

        public Task OnSubscribe(ISubscriber subscriber)
        {
            return Task.CompletedTask;
        }

        public Task OnUnsubscribe(ISubscriber subscriber)
        {
            UserModel user = _loginService.OnLogout(subscriber);

            string userLoginnotification = $"[SERVER] User {user.Name} has left the chat.";

            return _channelServices.BroadcastRawToAllButSenderAsync(System.Text.Encoding.UTF8.GetBytes(userLoginnotification), subscriber);
        }
    }
}
