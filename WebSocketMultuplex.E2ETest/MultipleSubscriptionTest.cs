using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using WebSocketMultiplexLib;
using WebSocketMultuplex.E2ETest.Utils;

namespace WebSocketMultuplex.E2ETest
{
   
    internal class MultipleSubscriptionTest
    {
        [Test]
        public async Task SubscribeToManyChannels()
        {
            Mock<IChannel> lettersChannel = new Mock<IChannel>();
            Mock<IChannel> numbersChannel = new Mock<IChannel>();

            string testLetters = "abcdef";
            string testNumbers = "123456";

            lettersChannel.Setup(c => c.OnSubscribe(It.IsAny<ISubscriber>())).Returns(Task.CompletedTask).Verifiable();
            numbersChannel.Setup(c => c.OnSubscribe(It.IsAny<ISubscriber>())).Returns(Task.CompletedTask).Verifiable();

            lettersChannel.Setup(c => c.OnChannelMessage(It.IsAny<ISubscriber>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Callback<ISubscriber, string>((sub, msg) =>
                {
                    Assert.That(msg, Is.EqualTo(testLetters));
                }).Verifiable();

            numbersChannel.Setup(c => c.OnChannelMessage(It.IsAny<ISubscriber>(), It.IsAny<string>()))
               .Returns(Task.CompletedTask)
               .Callback<ISubscriber, string>((sub, msg) =>
               {
                   Assert.That(msg, Is.EqualTo(testNumbers));
               }).Verifiable();

            IList<IChannel> channels = new List<IChannel>();
            channels.Add(lettersChannel.Object);
            channels.Add(numbersChannel.Object);

            TestApplication app = new TestApplication(channels);

            Task runApp = app.Run();
            using (app)
            {
                ClientWebSocket client = new ClientWebSocket();

                await client.ConnectAsync(new Uri(TestApplication.Url), CancellationToken.None);

                byte[] buffer = new byte[1024];

                WebSocketReceiveResult receiveResult = await client.ReceiveAsync(buffer, CancellationToken.None);

                Assert.That(receiveResult.Count, Is.GreaterThan(0));

                JArray obj = JArray.Parse(Encoding.UTF8.GetString(buffer, 0, receiveResult.Count));

                string lettersChannelName = obj[0].ToString();
                string numbersChannelName = obj[1].ToString();

                //subscribe to each channel
                await client.SendAsync(Encoding.UTF8.GetBytes(lettersChannelName), WebSocketMessageType.Text, true, CancellationToken.None);
                await client.SendAsync(Encoding.UTF8.GetBytes(numbersChannelName), WebSocketMessageType.Text, true, CancellationToken.None);

                //send a message to each channel
                await client.SendAsync(Encoding.UTF8.GetBytes(lettersChannelName + testLetters), WebSocketMessageType.Text, true, CancellationToken.None);
                await client.SendAsync(Encoding.UTF8.GetBytes(numbersChannelName + testNumbers), WebSocketMessageType.Text, true, CancellationToken.None);

                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }

            await runApp;

            lettersChannel.Verify(c => c.OnSubscribe(It.IsAny<ISubscriber>()), Times.Once);
            numbersChannel.Verify(c => c.OnSubscribe(It.IsAny<ISubscriber>()), Times.Once);

            lettersChannel.Verify(c => c.OnChannelMessage(It.IsAny<ISubscriber>(), It.IsAny<string>()), Times.Once);
            numbersChannel.Verify(c => c.OnChannelMessage(It.IsAny<ISubscriber>(), It.IsAny<string>()), Times.Once);
        }
    }
}
