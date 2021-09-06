using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;

namespace Services.Common.Caching
{
    public class DistributedCacheOptions
    {
        public DistributedCacheOptions()
        {
            KeyPrefix = "";

            CacheConfigurators = new List<Func<string, DistributedCacheEntryOptions>>();

            GlobalCacheEntryOptions = new DistributedCacheEntryOptions();
        }

        public string Host { get; set; }

        /// <summary>
        /// Throw or hide exceptions for the distributed cache.
        /// </summary>
        public bool HideErrors { get; set; } = true;

        /// <summary>
        /// Cache key prefix.
        /// </summary>
        public string KeyPrefix { get; set; }

        /// <summary>
        /// List of all cache configurators.
        /// (func argument:Name of cache)
        /// </summary>
        public List<Func<string, DistributedCacheEntryOptions>> CacheConfigurators { get; set; }

        /// <summary>
        /// Global Cache entry options.
        /// </summary>
        public DistributedCacheEntryOptions GlobalCacheEntryOptions { get; set; }
    }
}