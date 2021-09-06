using Microsoft.Extensions.DependencyInjection;

namespace EfCore.Audit
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAudit(this IServiceCollection services)
        {
            services.AddSingleton<IStorageInitializer, SqlServerStorageInitializer>();

            return services;
        }
    }
}