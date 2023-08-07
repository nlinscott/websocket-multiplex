using System.Threading.Tasks;

namespace WebSocketMultiplexLib
{
    public interface IChannel
    {
        void OnChannelReady(IChannelServices services);

        Task BeforeChannelShutdown();

        Task OnSubscribe(ISubscriber subscriber);

        Task OnUnsubscribe(ISubscriber subscriber);

        Task OnChannelMessage(ISubscriber sender, string message);
    }
}
