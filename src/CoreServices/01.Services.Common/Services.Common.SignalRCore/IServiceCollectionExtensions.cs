using Microsoft.Extensions.DependencyInjection;
using Services.Common.SignalRCore.Interfaces;
using Services.Common.SignalRCore.Options;

namespace Services.Common.SignalRCore
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddSignalRConnectionManager(this IServiceCollection services)
        {
            var options = AppSettings.Instance.Get<ConnectionManagerOptions>(nameof(ConnectionManagerOptions));

            switch (options.Provider)
            {
                case Models.Provider.Memory:

                    services.AddSingleton<IConnectionManager, InMemoryConnectionManager>();

                    break;

                case Models.Provider.Redis:

                    services.AddTransient<IConnectionManager, RedisCacheConnectionManager>();

                    break;
            }

            services.AddSingleton<IConnectionKeyNormalizer, ConnectionKeyNormalizer>();

            return services;
        }
    }
}