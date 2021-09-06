using Destructurama;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace Services.Common.APIs
{
    public static class APIProgramBase<T> where T : APIStartupBase
    {
        public static void Main(string[] args)
        {
            //Read Configuration from appSettings
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            // Initialize Logger
            Log.Logger = new LoggerConfiguration()
                .Destructure.UsingAttributes()
                .ReadFrom.Configuration(config) // Initializes the Serilog using the settings from app-settings.jon
                .Enrich.WithEnvironment("ASPNETCORE_ENVIRONMENT")
                .Enrich.WithMemoryUsage()
                .Enrich.WithClientIp()
                .Enrich.WithClientAgent()
                .Enrich.WithAssemblyName()
                .WriteTo.Seq(config["SeqLogUrl"])
                .CreateLogger();

            try
            {
                Log.Information("Application Starting.");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The Application failed to start.");
            }
            finally
            {
                Log.CloseAndFlush(); // Allows the logger to log any pending messages while the application closes down.
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .UseSerilog() // Uses Serilog instead of default .NET Logger.
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<T>();
                webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
            });
    }
}