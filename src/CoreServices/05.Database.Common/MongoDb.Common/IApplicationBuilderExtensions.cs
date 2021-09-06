using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using MongoDb.Common.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using MongoDB.Entities;
using Services.Common;
using System.Threading.Tasks;

namespace MongoDb.Common
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseInitMongoDb(this IApplicationBuilder app)
        {
            var mongoDbOptions = AppSettings.Instance.Get<MongoDbOptions>(nameof(MongoDbOptions));

            Task.Run(async () =>
            {
                await DB.InitAsync(mongoDbOptions.Prefix + '_' + mongoDbOptions.DatabaseName,
                    mongoDbOptions.HostName,
                    mongoDbOptions.Port);
            })
           .GetAwaiter()
           .GetResult();

            return app;
        }

        public static IApplicationBuilder UseInitMongoDbWithLog(this IApplicationBuilder app, ILogger logger)
        {
            var mongoDbOptions = AppSettings.Instance.Get<MongoDbOptions>(nameof(MongoDbOptions));

            var mongoConnectionUrl = new MongoUrl(mongoDbOptions.DatabaseConnection);

            var mongoClientSettings = MongoClientSettings.FromUrl(mongoConnectionUrl);

            mongoClientSettings.ClusterConfigurator = cb =>
            {
                cb.Subscribe<CommandStartedEvent>(e =>
                {
                    logger.LogInformation($"{e.CommandName} - {e.Command.ToJson()}");
                });
            };

            Task.Run(async () =>
            {
                await DB.InitAsync(mongoDbOptions.Prefix + '_' + mongoDbOptions.DatabaseName, mongoClientSettings);
            })
           .GetAwaiter()
           .GetResult();

            return app;
        }
    }
}