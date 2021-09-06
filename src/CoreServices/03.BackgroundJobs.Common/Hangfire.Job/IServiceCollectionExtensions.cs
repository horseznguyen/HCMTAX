using Hangfire.Job.Models;
using Hangfire.MemoryStorage;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Services.Common;
using Services.Common.ActionUtils;
using System;

namespace Hangfire.Job
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomHangfire(this IServiceCollection services, string sectionName = "")
        {
            var hangfireOptions = AppSettings.GetObject<HangfireOptions>(sectionName);

            return services.AddHangfire(hangfireOptions);
        }

        public static IServiceCollection AddHangfire(this IServiceCollection services)
        {
            return services.AddHangfire(_ => { });
        }

        public static IServiceCollection AddHangfire(this IServiceCollection services, HangfireOptions options)
        {
            return services.AddHangfire(_ =>
            {
                _.IsEnable = options.IsEnable;

                _.Url = options.Url;

                _.BackToUrl = options.BackToUrl;

                _.ConnectionStrings = options.ConnectionStrings;

                _.IsDisableJobDashboard = options.IsDisableJobDashboard;

                _.Provider = options.Provider;

                _.StatsPollingInterval = options.StatsPollingInterval;

                _.UnAuthorizeMessage = options.UnAuthorizeMessage;

                _.ExtendOptions = options.ExtendOptions;

                _.User = options.User;

                _.Pass = options.Pass;
            });
        }

        public static IServiceCollection AddHangfire(this IServiceCollection services, Action<HangfireOptions> configuration)
        {
            services.Configure(configuration);

            var options = configuration.GetValue();

            if (!options.IsEnable)
            {
                return services;
            }

            switch (options.Provider)
            {
                case HangfireProvider.Memory:
                    {
                        services.AddHangfire(config =>
                        {
                            config.UseMemoryStorage();

                            config.UseSerializerSettings(new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                            options.ExtendOptions?.Invoke(config, options);
                        });

                        GlobalConfiguration.Configuration.UseMemoryStorage();

                        options.ExtendOptions?.Invoke(GlobalConfiguration.Configuration, options);

                        break;
                    }
                case HangfireProvider.SqlServer:
                    {
                        services.AddHangfire(config =>
                        {
                            config.UseSqlServerStorage(options.ConnectionStrings, new SqlServerStorageOptions
                            {
                                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),

                                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),

                                QueuePollInterval = TimeSpan.Zero,

                                UseRecommendedIsolationLevel = true,

                                DisableGlobalLocks = true // Migration to Schema 7 is required
                            });

                            config.UseSerializerSettings(new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                            options.ExtendOptions?.Invoke(config, options);
                        });

                        GlobalConfiguration.Configuration.UseSqlServerStorage(options.ConnectionStrings);

                        options.ExtendOptions?.Invoke(GlobalConfiguration.Configuration, options);

                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return services;
        }
    }
}