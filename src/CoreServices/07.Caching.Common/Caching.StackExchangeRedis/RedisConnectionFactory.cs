using Services.Common;
using Services.Common.Caching;
using StackExchange.Redis;
using System;

namespace Caching.StackExchangeRedis
{
    public class RedisConnectionFactory
    {
        private static readonly Lazy<ConnectionMultiplexer> Connection;

        static RedisConnectionFactory()
        {
            var distributedCacheOptions = AppSettings.GetObject<DistributedCacheOptions>("");

            var options = ConfigurationOptions.Parse(distributedCacheOptions.Host);

            Connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }

        public static ConnectionMultiplexer GetConnection() => Connection.Value;
    }
}