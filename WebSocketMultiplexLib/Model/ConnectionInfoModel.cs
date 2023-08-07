using Microsoft.AspNetCore.Http;

namespace WebSocketMultiplexLib.Model
{
    internal sealed class ConnectionInfoModel : IConnectionInfo
    {
        public IHeaderDictionary Headers
        {
            get;
            set;
        }

        public ConnectionInfo Connection
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Headers.ToString() + "; " + Connection.ToString();
        }
    }
}
