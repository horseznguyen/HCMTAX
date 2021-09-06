using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EfCore.Audit
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAudit(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var sqlServerStorageInitializer = serviceProvider.GetService<IStorageInitializer>();

                sqlServerStorageInitializer.Initialize();
            }

            return app;
        }
    }
}