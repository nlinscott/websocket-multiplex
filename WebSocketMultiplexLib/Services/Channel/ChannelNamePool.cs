using Microsoft.Extensions.Logging;
using System;

namespace WebSocketMultiplexLib.Services.Channel
{
    /// <summary>
    /// Creates a two character name using every distinct combination of letters from the alphabet. <see cref="Constants.ChannelNameLength"/>
    /// </summary>
    internal sealed class ChannelNamePool : IChannelNamePool
    {
        
        private const string _alphabet = "abcdefghijklmnopqrstuvwxyz";
        private int _i = 0;
        private int _j = 0;

        private readonly ILogger _logger;

        public ChannelNamePool(ILogger<ChannelNamePool> logger)
        {
            _logger = logger;
        }

        public string Next()
        {
            char first = _alphabet[_i];
            char second = _alphabet[_j];

            if(_j == _alphabet.Length - 1)
            {
                if (_i == _alphabet.Length - 1)
                {
                    string message = $"Exceeding the limit for number of channels created: {Math.Pow(_alphabet.Length, 2)}";

                    Exception e = new System.Exception(message);

                    _logger.LogError(e, message);

                    throw e;
                }

                _i++;
                _j = 0;
            }
            else
            {
                _j++;
            }

            string name = $"{first}{second}";

            _logger.LogDebug("Creating channel with name: {Name}", name);

            return name;
        }
    }
}
