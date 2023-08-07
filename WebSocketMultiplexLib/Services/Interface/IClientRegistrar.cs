namespace WebSocketMultiplexLib.Services
{
    interface IClientRegistrar
    {
        void Add(ISubscriber sub, IWebSocketSubscriber socket);

        void Remove(ISubscriber sub);

        IWebSocketSubscriber Get(ISubscriber sub);
    }
}
