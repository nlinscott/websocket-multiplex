using System.Threading.Tasks;
using WebSocketMultiplexLib.Services.Channel;

namespace WebSocketMultiplexLib.Services
{
    internal sealed class MessageParser : IMessageParser
    {
        private readonly IChannelMessageRouter _router;

        public MessageParser(IChannelMessageRouter router)
        {
            _router = router;
        }

        public Task ParseAndRoute(ISubscriber sender, string message)
        {
            if(message.Length == Constants.ChannelNameLength)
            {
                //entire message is just the channel. Toggle subscription to that channel
                return _router.ToggleSubscription(sender, message);
            }

            string channelName = message.Substring(0, Constants.ChannelNameLength);
            string msg = message.Substring(Constants.ChannelNameLength, message.Length - Constants.ChannelNameLength);

            return _router.SendMessageToChannel(sender, channelName, msg);
        }
    }
}
