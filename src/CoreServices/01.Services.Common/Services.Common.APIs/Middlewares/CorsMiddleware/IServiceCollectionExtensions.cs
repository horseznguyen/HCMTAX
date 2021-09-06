using JetBrains.Annotations;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc.Cors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Services.Common.ActionUtils;
using System;
using System.Linq;

namespace Services.Common.APIs.Middlewares.CorsMiddleware
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCors(this IServiceCollection services)
        {
            return services.AddCors(_ => { });
        }

        public static IServiceCollection AddCors(this IServiceCollection services, [NotNull] Models.CorsOptions configuration)
        {
            return services.AddCors(_ =>
            {
                _.PolicyName = configuration.PolicyName;
                _.AllowOrigins = configuration.AllowOrigins;
                _.AllowHeaders = configuration.AllowHeaders;
                _.AllowMethods = configuration.AllowMethods;
                _.IsAllowCredentials = configuration.IsAllowCredentials;
            });
        }

        public static IServiceCollection AddCors(this IServiceCollection services, [NotNull] Action<Models.CorsOptions> configuration)
        {
            services.Configure(configuration);

            var options = configuration.GetValue();

            var corsBuilder = new CorsPolicyBuilder();

            if (options.IsOriginAllowed != null)
            {
                corsBuilder.SetIsOriginAllowed(options.IsOriginAllowed);
            }
            else if (options.AllowOrigins?.Any() == true)
            {
                options.AllowOrigins = options.AllowOrigins.Distinct().OrderBy(x => x).ToList();

                if (options.AllowOrigins.Contains("*"))
                {
                    corsBuilder.SetIsOriginAllowed(origin => true);
                }
                else
                {
                    corsBuilder.WithOrigins(options.AllowOrigins.ToArray());

                    corsBuilder.SetIsOriginAllowedToAllowWildcardSubdomains();
                }
            }

            if (options.AllowHeaders?.Any() == true)
            {
                if (options.AllowHeaders.Contains("*"))
                {
                    corsBuilder.AllowAnyHeader();
                }
                else
                {
                    corsBuilder.WithHeaders(options.AllowHeaders.ToArray());
                }
            }

            if (options.AllowMethods?.Any() == true)
            {
                if (options.AllowMethods.Contains("*"))
                {
                    corsBuilder.AllowAnyMethod();
                }
                else
                {
                    corsBuilder.WithMethods(options.AllowMethods.ToArray());
                }
            }

            if (options.IsAllowCredentials)
            {
                corsBuilder.AllowCredentials();
            }
            else
            {
                corsBuilder.DisallowCredentials();
            }

            options.ExtendPolicyBuilder?.Invoke(corsBuilder);

            services.AddCors(config =>
            {
                config.DefaultPolicyName = options.PolicyName;

                config.AddDefaultPolicy(corsBuilder.Build());

                options.ExtendPolicyOptions?.Invoke(config);
            });

            services.TryAddTransient<CorsAuthorizationFilter, CorsAuthorizationFilter>();

            return services;
        }
    }
}