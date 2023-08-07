using Microsoft.AspNetCore.TestHost;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Net.WebSockets;
using System.Text;
using WebSocketMultiplexLib;
using WebSocketMultuplex.E2ETest.Utils;

namespace WebSocketMultuplex.E2ETest
{

    public class ChannelListTests
    {
        private class TestChannel : IChannel
        {
            private readonly ArraySegment<byte> _testEcho;

            private IChannelServices _services;

            public TestChannel(string testEcho)
            {
                _services = null;
                _testEcho = Encoding.UTF8.GetBytes(testEcho);
            }

            public Task BeforeChannelShutdown()
            {
                return Task.CompletedTask;
            }

            public Task OnChannelMessage(ISubscriber sender, string message)
            {
                return Task.CompletedTask;
            }

            public void OnChannelReady(IChannelServices services)
            {
                _services = services;
            }

            public Task OnSubscribe(ISubscriber subscriber)
            {
                return _services.SendRawToAsync(subscriber, _testEcho);
            }

            public Task OnUnsubscribe(ISubscriber subscriber)
            {
                return _services.SendRawToAsync(subscriber, _testEcho);
            }
        }

        [Test]
        public async Task SubscribeSuccess_UnsubscribeSameClient()
        {
            string testEcho = "test data abc123";

            IList<IChannel> channels = new List<IChannel>();
            channels.Add(new TestChannel(testEcho));

            TestApplication app = new TestApplication(channels);

            Task runApp = app.Run();

            using (app)
            {
                ClientWebSocket client = new ClientWebSocket();

                await client.ConnectAsync(new Uri(TestApplication.Url), CancellationToken.None);

                byte[] buffer = new byte[1024];

                WebSocketReceiveResult receiveResult = await client.ReceiveAsync(buffer, CancellationToken.None);

                Assert.That(receiveResult.Count, Is.GreaterThan(0));

                Assert.That(receiveResult.MessageType, Is.EqualTo(WebSocketMessageType.Text));

                JArray obj = JArray.Parse(Encoding.UTF8.GetString(buffer, 0, receiveResult.Count));

                string testChannelName = obj[0].ToString();

                await client.SendAsync(Encoding.UTF8.GetBytes(testChannelName), WebSocketMessageType.Text, true, CancellationToken.None);

                receiveResult = await client.ReceiveAsync(buffer, CancellationToken.None);

                string data = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);

                Assert.That(data, Is.EqualTo(testEcho));

                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }

            await runApp;
        }

    }
}