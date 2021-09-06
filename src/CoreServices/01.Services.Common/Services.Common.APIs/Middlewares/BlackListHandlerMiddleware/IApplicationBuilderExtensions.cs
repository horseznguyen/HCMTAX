using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Services.Common.Authorization;
using Services.Common.Authorization.Model;
using Services.Common.Caching.Interfaces;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Common.APIs.Middlewares.BlackListHandlerMiddleware
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBlackListHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<BlackListHandlerMiddleware>();

            return app;
        }
    }

    public class BlackListHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IDistributedCache<UserItemCache> _distributedCache;

        public BlackListHandlerMiddleware(RequestDelegate next, IDistributedCache<UserItemCache> distributedCache)
        {
            _next = next;

            _distributedCache = distributedCache;
        }

        public async Task Invoke(HttpContext context)
        {
            var userIdClaim = context.User.Claims.SingleOrDefault(x => x.Type == ClaimsTypeName.USERID);

            var sessionCode = context.User.Claims.SingleOrDefault(x => x.Type == ClaimsTypeName.SESSION_CODE);

            if (userIdClaim != null && sessionCode != null)
            {
                var userCacheItem = await _distributedCache.GetAsync(userIdClaim.Value, true, CancellationToken.None);

                if (userCacheItem != null && (userCacheItem.IsPermissionChanged || userCacheItem.ListOfSessionCodeInValid.Any(x => x == sessionCode.Value)))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                    if (!context.Response.HasStarted)
                    {
                        await context.Response.CompleteAsync();
                    }
                    return;
                }
            }
            await _next(context);
        }
    }
}