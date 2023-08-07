using System.Threading.Tasks;

namespace WebSocketMultiplexLib.Services
{
    interface IMessageParser
    {
        Task ParseAndRoute(ISubscriber sender, string message);
    }
}
