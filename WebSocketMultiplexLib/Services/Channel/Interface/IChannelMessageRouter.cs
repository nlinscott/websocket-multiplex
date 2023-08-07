using System.Threading.Tasks;

namespace WebSocketMultiplexLib.Services.Channel
{
    interface IChannelMessageRouter
    {
        Task ToggleSubscription(ISubscriber sender, string channelName);

        Task SendMessageToChannel(ISubscriber sender, string channelName, string message);

        Task UnsubscribeFromAll(ISubscriber sender);
    }
}
