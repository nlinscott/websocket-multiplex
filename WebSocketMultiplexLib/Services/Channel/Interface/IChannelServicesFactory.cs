namespace WebSocketMultiplexLib.Services.Channel
{
    internal interface IChannelServicesFactory
    {
        IChannelServices Create(string channelName, ISubscriberCollection subscribers);
    }
}
