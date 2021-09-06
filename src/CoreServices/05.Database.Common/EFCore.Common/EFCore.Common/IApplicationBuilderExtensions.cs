using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.Common;

namespace EFCore.Common
{
    public static class IApplicationBuilderExtensions
    {
        public static IServiceCollection AddCustomDbContext<TContextImplementation>(this IServiceCollection services, string sectionName = "") where TContextImplementation : DbContext, IDbContext
        {
            var options = AppSettings.GetObject<EfCoreOptions>(sectionName);

            services.Configure<EfCoreOptions>(_ =>
            {
                _.CommandTimeOut = options.CommandTimeOut;

                _.ConnectionStrings = options.ConnectionStrings;

                _.DateTimeKind = options.DateTimeKind;

                _.MaxRetryCount = options.MaxRetryCount;

                _.MaxRetryDelayInSecond = options.MaxRetryDelayInSecond;
            });

            services.AddDbContext<IDbContext, TContextImplementation>(optionsBuilder =>
            {
                optionsBuilder
                    .UseSqlServer(options.ConnectionStrings, opt =>
                    {
                        opt.CommandTimeout(options.CommandTimeOut);
                    });
            });

            return services;
        }
    }
}