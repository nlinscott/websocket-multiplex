namespace WebSocketMultiplexLib.Services
{
    public interface IDisconnectService
    {
        void DisconnectSession(ISubscriber sub);
    }
}
