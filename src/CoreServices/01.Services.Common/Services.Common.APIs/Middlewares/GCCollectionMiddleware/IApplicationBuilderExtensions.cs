using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Services.Common.APIs.Middlewares.GCCollectionMiddleware
{
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Release memory if possible by GC Collection.<br/>
        /// Keep this Middleware at the top the pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseGCCollect(this IApplicationBuilder app)
        {
            app.UseMiddleware<GcCollectMiddleware>();

            return app;
        }
    }

    public class GcCollectMiddleware
    {
        private readonly RequestDelegate _next;

        public GcCollectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            GC.Collect(2, GCCollectionMode.Forced, true);

            GC.WaitForPendingFinalizers();
        }
    }
}