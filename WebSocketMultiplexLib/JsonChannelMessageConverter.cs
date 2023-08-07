using System.Text.Json;

namespace WebSocketMultiplexLib
{
    internal sealed class JsonChannelMessageConverter : IChannelMessageConverter
    {
        public byte[] ConvertToByteArray(object obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }
    }
}
