using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using WebSocketMultiplexLib.Services.Channel;

namespace WebSocketMultiplexLib.UnitTests
{
    internal class ChannelNamePoolTests
    {
        [Test]
        public void Next_GetsValidName()
        {
            ChannelNamePool namePool = new ChannelNamePool(Mock.Of<ILogger<ChannelNamePool>>());

            string channelName = namePool.Next();

            Assert.That(channelName, Is.EqualTo("aa"));
        }

        [Test]
        public void Next_ExceedsNameLimit()
        {
            ChannelNamePool namePool = new ChannelNamePool(Mock.Of<ILogger<ChannelNamePool>>());

            double max = Math.Pow(26, 2);

            Assert.DoesNotThrow(() =>
            {
                for (double i = 0; i < max - 1; i++)
                {
                    _ = namePool.Next();
                }
            });

            Assert.Throws<Exception>(() =>
            {
               _ = namePool.Next();
            });
        }
    }
}