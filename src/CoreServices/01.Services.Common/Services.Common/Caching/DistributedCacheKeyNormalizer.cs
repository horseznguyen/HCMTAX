using Microsoft.Extensions.Options;
using Services.Common.Caching.Interfaces;
using Services.Common.DI;

namespace Services.Common.Caching
{
    [TransientDependency(ServiceType = typeof(IDistributedCacheKeyNormalizer))]
    public class DistributedCacheKeyNormalizer : IDistributedCacheKeyNormalizer
    {
        protected DistributedCacheOptions DistributedCacheOptions { get; }

        public DistributedCacheKeyNormalizer(IOptions<DistributedCacheOptions> distributedCacheOptions)
        {
            DistributedCacheOptions = distributedCacheOptions.Value;
        }

        public virtual string NormalizeKey(DistributedCacheKeyNormalizeArgs args)
        {
            var normalizeKey = $"c:{args.CacheName},k:{DistributedCacheOptions.KeyPrefix}{args.Key}";

            return normalizeKey;
        }
    }
}