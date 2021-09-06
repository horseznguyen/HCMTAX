using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Services.Common.APIs.Middlewares.RequestRewindMiddleware
{
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        ///     Enable Rewind help to get Request Body content.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequestRewind(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestRewindMiddleware>();
        }
    }

    public class RequestRewindMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestRewindMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            // Allows using several time the stream in ASP.Net Core. Enable Rewind help to get
            // Request Body content.
            // https://github.com/aspnet/AspNetCore/issues/12505: Change EnableRewind to EnableBuffering
            context.Request.EnableBuffering();

            return _next(context);
        }
    }
}