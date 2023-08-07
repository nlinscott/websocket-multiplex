using Microsoft.AspNetCore.Http;

namespace WebSocketMultiplexLib
{
    public interface IConnectionInfo
    {
        IHeaderDictionary Headers { get; }

        ConnectionInfo Connection { get; }
    }
}
