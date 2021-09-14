using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Services.Common.APIs;
using Services.Common.APIs.Middlewares.ExceptionHandlerMiddleware;
using Services.Common.APIs.Middlewares.GCCollectionMiddleware;
using Services.Common.APIs.Middlewares.LocalizationMiddleware;
using Services.Common.APIs.Middlewares.SwaggerMiddleware;
using Services.Common.DI;
using Services.Common.ExceptionsModels;
using System.Linq;

namespace HCMTAX.API
{
    public class Startup : APIStartupBase
    {
        public Startup(IWebHostEnvironment webHostEnvironment) : base(webHostEnvironment)
        {
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCustomSwagger();
            app.UseGCCollect();
            app.UseCustomExceptionHandler();
            app.UseSerilogRequestLogging();
            app.UseRequestResponseLogging();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCustomLocalization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var moduleTypes = services.AddDI<APIModule>().ToList();

            if (moduleTypes == null || !moduleTypes.Any())
            {
                throw new InitializationException("Not found modules.");
            }

            services
                .AddOptions()
                .AddHttpContextAccessor()
                .AddCustomApiVersion(null)
                .AddCustomSwagger(moduleTypes)
                .AddJwtAuthentication()
                .AddControllers();
        }
    }
}