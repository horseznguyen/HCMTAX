using Microsoft.AspNetCore.Builder;

namespace Services.Common.APIs.Middlewares.SwaggerMiddleware
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger(); // Enable middleware to serve swagger-ui(HTML, JS, CSS, etc.),

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ASC.BE API");
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            });

            return app;
        }
    }
}