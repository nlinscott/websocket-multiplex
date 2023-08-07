namespace WebSocketMultiplexLib
{
    public interface IChannelMessageConverter
    {
        byte[] ConvertToByteArray(object obj);
    }
}
