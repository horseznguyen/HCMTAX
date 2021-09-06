using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Services.Common.APIs.Middlewares.CorsMiddleware.Models;

namespace Services.Common.APIs.Middlewares.CorsMiddleware
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCors(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetService<IOptions<CorsOptions>>().Value;

            app.UseCors(options.PolicyName);

            return app;
        }
    }
}