using ChatApp.Shared.Model;
using WebSocketMultiplexLib;

namespace WebSocketMultiplex.Services
{
    internal interface ILoginService
    {
        void OnLogin(ISubscriber sub, UserModel model);

        UserModel OnLogout(ISubscriber sub);
    }
}
