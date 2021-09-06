using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Common;
using Services.Common.ActionUtils;
using Services.Common.Caching;
using System;

namespace Caching.StackExchangeRedis
{
    public static class IApplicationBuilderExtensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, string sectionName = "")
        {
            var options = AppSettings.GetObject<DistributedCacheOptions>(sectionName);

            return services.AddRedisCache(options);
        }

        public static IServiceCollection AddRedisCache(this IServiceCollection services, [NotNull] DistributedCacheOptions configuration)
        {
            return services.AddRedisCache(_ =>
            {
                _.Host = configuration.Host;

                _.KeyPrefix = configuration.KeyPrefix;

                _.HideErrors = configuration.HideErrors;
            });
        }

        public static IServiceCollection AddRedisCache(this IServiceCollection services, [NotNull] Action<DistributedCacheOptions> configuration)
        {
            services.Configure(configuration);

            var options = configuration.GetValue();

            services.AddStackExchangeRedisCache(opt => { opt.Configuration = options.Host; });

            return services;
        }
    }
}