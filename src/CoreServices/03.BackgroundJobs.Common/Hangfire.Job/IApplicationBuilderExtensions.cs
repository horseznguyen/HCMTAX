using Hangfire.Job.Models;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Services.Common.CheckUtils;
using System;

namespace Hangfire.Job
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomHangfire(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetService<IOptions<HangfireOptions>>().Value;

            if (!options.IsEnable) return app;

            if (!options.IsDisableJobDashboard)
            {
                CheckHelper.CheckNullOrWhiteSpace(options.Url, nameof(options.Url));

                app.UseHangfireDashboard(options.Url, new DashboardOptions
                {
                    Authorization = new[] { new HangfireCustomBasicAuthenticationFilter
                    {
                        User = options.User,
                        Pass = options.Pass
                    }},

                    AppPath = options.BackToUrl,

                    StatsPollingInterval = options.StatsPollingInterval
                });
            }

            app.UseHangfireServer();

            return app;
        }

        public static IApplicationBuilder UseCustomHangfire(this IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            // Configure hangfire to use the new JobActivator we defined.
            GlobalConfiguration.Configuration.UseActivator(new HangfireActivator(serviceProvider));

            app.UseCustomHangfire();

            return app;
        }
    }
}