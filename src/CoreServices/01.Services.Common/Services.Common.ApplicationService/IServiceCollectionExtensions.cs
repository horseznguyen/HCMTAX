using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Services.Common.ApplicationService.Behaviors;

namespace Services.Common.ApplicationService
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));

            return services;
        }
    }
}