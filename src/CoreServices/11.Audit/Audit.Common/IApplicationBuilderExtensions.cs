using Audit.Core;
using Audit.Redis.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Services.Common;
using Services.Common.RunTime;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Audit.Common
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAuditRedisProviderPubSubWithPolly(this IApplicationBuilder app, ILogger logger)
        {
            var options = AppSettings.GetObject<AuditOptions>(nameof(AuditOptions));

            Random rd = new();

            var redisDataProvider = new RedisProviderPubSub(options.ConnectionString, null, env => $"{options.Prefix}:{options.ChannelName}");

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(options.RetryCount, retryAttempt =>
                {
                    var timeToWait = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(rd.Next(0, 1000));

                    logger.LogWarning($"Waiting {timeToWait.TotalSeconds} seconds");

                    return timeToWait;
                });

            var retryPolicyAsync = Policy
               .Handle<Exception>()
               .WaitAndRetryAsync(options.RetryCount, retryAttempt =>
               {
                   var timeToWait = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(rd.Next(0, 1000));

                   logger.LogWarning($"Waiting {timeToWait.TotalSeconds} seconds");

                   return timeToWait;
               });

            var fallbackPolicy = Policy<object>
                .Handle<Exception>()
                .Fallback(
                    fallbackAction: (context) =>
                    {
                        var auditEvent = context["event"] as AuditEvent;

                        logger.LogError(auditEvent.ToJson());

                        return -1;
                    },
                    onFallback: (ex, ctx) => { })
                .Wrap(retryPolicy);

            var fallbackAsyncPolicy = Policy<object>
                .Handle<Exception>()
                .FallbackAsync(
                    fallbackAction: (Context context, CancellationToken ct) =>
                    {
                        var auditEvent = context["event"] as AuditEvent;

                        logger.LogError(auditEvent.ToJson());

                        return Task.FromResult((object)-1);
                    },
                    onFallbackAsync: async (r, c) => await Task.CompletedTask)
                .WrapAsync(retryPolicyAsync);

            Configuration.Setup()
                .UseCustomProvider(new PollyRedisDataProvider(redisDataProvider, fallbackPolicy, fallbackAsyncPolicy))
                .WithCreationPolicy(EventCreationPolicy.InsertOnEnd);

            return app;
        }

        public static IApplicationBuilder UseAuditRedisProviderPubSub(this IApplicationBuilder app)
        {
            var options = AppSettings.GetObject<AuditOptions>(nameof(AuditOptions));

            // Send audits to PubSub channels.
            Configuration.Setup()
                .UseRedis(redis => redis
                    .ConnectionString(options.ConnectionString)
                    .AsPubSub(pub => pub
                        .Channel(ev => $"{options.Prefix}:{options.ChannelName}")));

            return app;
        }

        public static IApplicationBuilder UseAddExtraInfomationToEvent(this IApplicationBuilder app, IHttpContextAccessor contextAccessor)
        {
            Configuration.AddCustomAction(ActionType.OnScopeCreated, scope =>
            {
                if (contextAccessor.HttpContext != null) // if tasks run in the background. If will be not throw exception.
                {
                    var userSessionInfo = (IUserSessionInfo)contextAccessor.HttpContext.RequestServices.GetService(typeof(IUserSessionInfo));

                    if (userSessionInfo != null && userSessionInfo.UserId != null)
                    {
                        scope.SetCustomField(nameof(userSessionInfo.UserId), userSessionInfo.UserId);
                    }
                }
            });

            return app;
        }
    }
}