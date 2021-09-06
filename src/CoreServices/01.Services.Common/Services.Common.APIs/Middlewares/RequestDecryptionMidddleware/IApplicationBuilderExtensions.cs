using Microsoft.AspNetCore.Http;
using Services.Common.SecurityUtils;
using System;
using System.Threading.Tasks;

namespace Services.Common.APIs.Middlewares.EncryptionMidddleware
{
    public class IApplicationBuilderExtensions
    {
    }

    public class RequestDecryptionMidddleware
    {
        private readonly RequestDelegate _next;

        public RequestDecryptionMidddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            string password = AppSettings.Instance.Get<string>("Key");

            if (env == "Production")
            {
                httpContext.Request.Body = SecurityHelper.DecryptStream(httpContext.Request.Body, password);
            }

            await _next(httpContext);
        }
    }
}