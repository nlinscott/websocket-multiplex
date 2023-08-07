using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WebSocketMultiplexLib.Middleware;
using WebSocketMultiplexLib.Services;
using WebSocketMultiplexLib.Services.Channel;

namespace WebSocketMultiplexLib
{
    public static class WebSocketMultiplexingExtensions
    {
        /// <summary>
        /// Adds multiplexing middleware
        /// </summary>
        public static IApplicationBuilder UseMultiplexing(this IApplicationBuilder app)
        {
            //order matters here - this is a virual middleware pipeline
            app.UseMiddleware<WebSocketFilter>();

            SetupChannels(app.ApplicationServices);

            return app;
        }

        private static void SetupChannels(IServiceProvider serviceProvider)
        {
            ChannelCacheInitializer init = serviceProvider.GetRequiredService<ChannelCacheInitializer>();

            IEnumerable<IChannel> channels = serviceProvider.GetServices<IChannel>();
            if (channels == null || !channels.Any())
            {
                string error = "No channels configured. you must implement IChannel and add your implementation to the service collection.";
                serviceProvider.GetRequiredService<ILogger<object>>().LogError(error);
                throw new System.Exception(error);
            }

            init.PopulateChannelCache(serviceProvider, channels);
        }

        /// <summary>
        /// Adds required services for multiplexing
        /// </summary>
        public static IServiceCollection AddMultiplexing(this IServiceCollection collection)
        {
            collection.AddSingleton<IClientRegistrar, ClientRegistrar>();
            collection.AddSingleton<IListenerService, ClientListenerService>();
            collection.AddSingleton<IChannelMessageRouter, ChannelMessageRouter>();
            collection.AddSingleton<IChannelSerializer, ChannelSerializer>();
            collection.AddSingleton<IMessageParser, MessageParser>();
            collection.AddSingleton<ConnectionStateService>();
            collection.AddSingleton<IConnectionStateService>(sp =>
            {
                return sp.GetRequiredService<ConnectionStateService>();
            });

            collection.AddSingleton<IDisconnectService>(sp =>
            {
                return sp.GetRequiredService<ConnectionStateService>();
            });

            collection.AddSingleton<IChannelNamePool, ChannelNamePool>();
            collection.AddSingleton<IChannelServicesFactory, ChannelServicesFactory>();

            //forwarding using factory methods
            collection.AddSingleton<ChannelCache>();
            collection.AddSingleton<IChannelCache>(sp => { return sp.GetRequiredService<ChannelCache>(); });
            collection.AddSingleton<ICache<IServerChannel>>(sp => { return sp.GetRequiredService<ChannelCache>(); });

            collection.AddSingleton(sp =>
            {
                return new HashSet<IServerChannel>(ServerChannelComparer.Instance);
            });

            collection.AddTransient<ISubscriberCollection>(sp =>
            {
                return new ThreadSafeSubscriberCollection(new HashSet<ISubscriber>(SubscriberKeyComparer.Instance));
            });

            collection.AddTransient<ChannelCacheInitializer>();

            return collection;
        }
    }
}
