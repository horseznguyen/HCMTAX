using Microsoft.AspNetCore.Builder;

namespace Services.Common.APIs.Middlewares.SecurityHeadersMiddleware
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            app.UseXContentTypeOptions();

            app.UseReferrerPolicy(opts => opts.NoReferrer());

            app.UseXfo(options => options.Deny());

            return app;
        }
    }
}