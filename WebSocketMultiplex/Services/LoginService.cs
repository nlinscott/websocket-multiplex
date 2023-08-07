using ChatApp.Shared.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using WebSocketMultiplexLib;

namespace WebSocketMultiplex.Services
{
    internal class LoginService : ILoginService
    {
        private readonly ConcurrentDictionary<ISubscriber, UserModel> _logins = new ConcurrentDictionary<ISubscriber, UserModel>(SubscriberKeyComparer.Instance);

        private readonly ILogger _logger;

        public LoginService(ILogger<LoginService> logger)
        {
            _logger = logger;
        }

        public void OnLogin(ISubscriber sub, UserModel model)
        {
            if(!_logins.ContainsKey(sub))
            {
                _logins.TryAdd(sub, model);
            }
        }

        public UserModel OnLogout(ISubscriber sub)
        {
            if(_logins.TryRemove(sub, out UserModel user))
            {
                _logger.LogInformation($"Logged out user: {sub.ConnectionInfo}");

                return user;
            }

            return null;
        }

        public UserModel GetUser(ISubscriber sub)
        {
            if(_logins.TryGetValue(sub, out UserModel user))
            {
                return user;
            }

            _logger.LogWarning($"Unable to find logged in user: {sub.ConnectionInfo}");
            return null;
        }
    }
}
