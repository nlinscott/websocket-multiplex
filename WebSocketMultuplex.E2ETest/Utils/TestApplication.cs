using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebSocketMultiplexLib;
using System.Net;

namespace WebSocketMultuplex.E2ETest.Utils
{
    internal sealed class TestApplication : IDisposable
    {

        public const string Url = "ws://localhost:9000";

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private readonly IWebHost _webHost;

        public TestApplication(IEnumerable<IChannel> channels)
        {
            _webHost = WebHost.CreateDefaultBuilder()
                     .UseKestrel(k =>
                     {
                         k.ListenLocalhost(9000, opts =>
                         {
                             opts.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
                         });
                     })
                     .ConfigureServices(services =>
                     {
                         services.AddLogging(log =>
                         {
                             log.AddConsole();
                         });

                         services.AddMultiplexing();

                         services.AddSingleton(channels);
                     })
                    .Configure(app =>
                    {
                        app.UseWebSockets();
                        app.UseMultiplexing();

                    }).Build();
        }


        public Task Run()
        {
            return _webHost.RunAsync(_cts.Token);
        }

        #region IDisposable
        private bool disposedValue;

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        _cts.Cancel();
                        _cts.Dispose();

                        _webHost.Dispose();
                    }
                    catch { }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
