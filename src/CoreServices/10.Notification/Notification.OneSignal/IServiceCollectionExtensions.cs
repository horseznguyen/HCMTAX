using Elect.Notification.OneSignal.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Notification.OneSignal.Interfaces;
using Notification.OneSignal.Models;
using Notification.OneSignal.Services;
using System;

namespace Notification.OneSignal
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddElectOneSignal(this IServiceCollection services, [NotNull] OneSignalOptions configuration)
        {
            return services.AddElectOneSignal(_ =>
            {
                _.AuthKey = configuration.AuthKey;
                _.Apps = configuration.Apps;
            });
        }

        public static IServiceCollection AddElectOneSignal(this IServiceCollection services, [NotNull] Action<OneSignalOptions> configuration)
        {
            services.Configure(configuration);

            services.TryAddScoped<IOneSignalApp, OneSignalApp>();

            services.TryAddScoped<IOneSignalDevice, OneSignalDevice>();

            services.TryAddScoped<IOneSignalNotification, OneSignalNotification>();

            services.TryAddScoped<IOneSignalClient, OneSignalClient>();

            return services;
        }
    }
}