namespace WebSocketMultiplexLib
{
    public interface ISubscriber
    {
        IConnectionInfo ConnectionInfo
        {
            get;
        }

        string SessionId
        {
            get;
        }
    }
}
