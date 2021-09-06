using Dapper.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Services.Common;

namespace Dapper.Common
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDapper(this IServiceCollection services, string sectionName = "")
        {
            var dapperOptions = AppSettings.GetObject<DapperOptions>(sectionName);

            services.Configure<DapperOptions>(_ =>
            {
                _.ConnectionStrings = dapperOptions.ConnectionStrings;

                _.CommandTimeOut = dapperOptions.CommandTimeOut;
            });

            services.AddScoped<IDapperDbConnectionFactory, DapperDbConnectionFactory>();

            return services;
        }
    }
}