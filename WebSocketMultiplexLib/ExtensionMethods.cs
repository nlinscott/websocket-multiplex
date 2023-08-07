using System.Collections.Generic;

namespace WebSocketMultiplexLib
{
    internal static class ExtensionMethods
    {
        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            yield return item;
        }
    }
}
