using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebSocketMultiplex.Services;
using WebSocketMultiplexLib;

namespace WebSocketMultiplex
{
    internal sealed class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(log =>
            {
                log.AddConsole();
            });

            services.AddSingleton<ILoginService, LoginService>();

            services.AddSingleton<IChannel, LoginChannel>();
            services.AddSingleton<IChannel, ChatChannel>();

            services.AddMultiplexing();
        }

        public void Configure(IApplicationBuilder builder)
        {
            builder.UseWebSockets();
            builder.UseMultiplexing();
        }
    }
}
