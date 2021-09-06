using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Common.ExceptionsModels;
using Services.Common.MethodResultUtils;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Services.Common.APIs.Middlewares.ExceptionHandlerMiddleware
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlerMiddleware>();

            return app;
        }
    }

    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        private readonly IWebHostEnvironment _env;

        public ExceptionHandlerMiddleware(RequestDelegate next, IWebHostEnvironment env, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;

            _logger = logger;

            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleUnhandledExceptionAsync(httpContext, ex).ConfigureAwait(false);
            }
        }

        private async Task HandleUnhandledExceptionAsync(HttpContext context, Exception exception)
        {
            if (!context.Response.HasStarted)
            {
                int statusCode;

                var result = new VoidMethodResult();

                context.Response.Clear();

                context.Response.ContentType = "application/json";

                if (exception.GetType().BaseType == typeof(BaseException))
                {
                    var baseException = (BaseException)exception;

                    statusCode = baseException.HttpStatusCode;

                    result.AddErrorMessages(baseException.ErrorResultList);
                }
                else
                {
                    statusCode = (int)HttpStatusCode.InternalServerError; // 500

                    result.AddErrorMessage(ErrorHelpers.GetExceptionMessage(exception), exception.StackTrace);

                    _logger.LogError(exception, exception.Message);
                }

                context.Response.StatusCode = statusCode;

                // If we log too many things, the performance will be affected.
                // Do not log everything, just to log the critical information that will help you diagnose a fault
                // in the production environment.
                if (_env.IsProduction() && statusCode == (int)HttpStatusCode.InternalServerError)
                {
                    return;
                }

                await context.Response.WriteAsync(result.ToJsonString()).ConfigureAwait(false);
            }
        }
    }
}