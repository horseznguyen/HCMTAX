using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace Services.Common.APIs
{
    public abstract class APIStartupBase
    {
        protected IConfiguration Configuration { get; set; }
        protected IWebHostEnvironment WebHostEnvironment { get; set; }

        protected APIStartupBase(IWebHostEnvironment webHostEnvironment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(webHostEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{webHostEnvironment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            Configuration = builder.Build(); // load all file config to Configuration property

            AppSettings.Instance.SetConfiguration(Configuration);

            WebHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }
    }
}