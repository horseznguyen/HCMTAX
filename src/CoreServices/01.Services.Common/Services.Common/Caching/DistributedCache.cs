using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Common.Caching.Interfaces;
using Services.Common.DI;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Common.Caching
{
    [SingletonDependency(ServiceType = typeof(IDistributedCache<>))]
    public class DistributedCache<TCacheItem> : DistributedCache<TCacheItem, string>, IDistributedCache<TCacheItem> where TCacheItem : class
    {
        public DistributedCache(IOptions<DistributedCacheOptions> distributedCacheOption,
            IDistributedCache cache,
            IDistributedCacheSerializer serializer,
            IDistributedCacheKeyNormalizer keyNormalizer,
            ILogger<DistributedCache<TCacheItem, string>> logger) : base(distributedCacheOption, cache, serializer, keyNormalizer, logger)
        {
        }
    }

    [SingletonDependency(ServiceType = typeof(IDistributedCache<,>))]
    public class DistributedCache<TCacheItem, TCacheKey> : IDistributedCache<TCacheItem, TCacheKey> where TCacheItem : class
    {
        #region Fields

        private readonly DistributedCacheOptions _distributedCacheOption;
        public ILogger<DistributedCache<TCacheItem, TCacheKey>> Logger { get; }
        protected string CacheName { get; set; }
        protected IDistributedCache Cache { get; }
        protected IDistributedCacheSerializer Serializer { get; }
        protected IDistributedCacheKeyNormalizer KeyNormalizer { get; }

        protected DistributedCacheEntryOptions DefaultCacheOptions;

        #endregion Fields

        #region Constructors

        public DistributedCache(
            IOptions<DistributedCacheOptions> distributedCacheOption,
            IDistributedCache cache,
            IDistributedCacheSerializer serializer,
            IDistributedCacheKeyNormalizer keyNormalizer, ILogger<DistributedCache<TCacheItem, TCacheKey>> logger)
        {
            _distributedCacheOption = distributedCacheOption.Value;
            Cache = cache;
            Serializer = serializer;
            KeyNormalizer = keyNormalizer;
            Logger = logger;

            SetDefaultOptions();
        }

        #endregion Constructors

        #region Get

        public virtual TCacheItem Get(TCacheKey key, bool? hideErrors = null)
        {
            hideErrors ??= _distributedCacheOption.HideErrors;

            byte[] cachedBytes;

            try
            {
                cachedBytes = Cache.Get(NormalizeKey(key));
            }
            catch (Exception ex)
            {
                if (hideErrors == true)
                {
                    HandleException(ex);
                    return null;
                }

                throw;
            }

            return ToCacheItem(cachedBytes);
        }

        public async Task<TCacheItem> GetAsync(TCacheKey key, bool? hideErrors = null, CancellationToken token = default)
        {
            hideErrors ??= _distributedCacheOption.HideErrors;

            byte[] cachedBytes;

            try
            {
                cachedBytes = await Cache.GetAsync(NormalizeKey(key), token);
            }
            catch (Exception ex)
            {
                if (hideErrors == true)
                {
                    HandleException(ex);

                    return null;
                }

                throw;
            }

            if (cachedBytes == null)
            {
                return null;
            }

            return Serializer.Deserialize<TCacheItem>(cachedBytes);
        }

        #endregion Get

        #region Set

        public virtual void Set(TCacheKey key, TCacheItem value, DistributedCacheEntryOptions options = null, bool? hideErrors = null)
        {
            try
            {
                hideErrors ??= _distributedCacheOption.HideErrors;

                Cache.Set(NormalizeKey(key), Serializer.Serialize(value), options ?? DefaultCacheOptions);
            }
            catch (Exception ex)
            {
                if (hideErrors == true)
                {
                    HandleException(ex);
                    return;
                }

                throw;
            }
        }

        public virtual async Task SetAsync(TCacheKey key, TCacheItem value, DistributedCacheEntryOptions options = null, bool? hideErrors = null, CancellationToken token = default)
        {
            try
            {
                hideErrors ??= _distributedCacheOption.HideErrors;

                await Cache.SetAsync(NormalizeKey(key), Serializer.Serialize(value), options ?? DefaultCacheOptions, token);
            }
            catch (Exception ex)
            {
                if (hideErrors == true)
                {
                    HandleException(ex);
                    return;
                }

                throw;
            }
        }

        #endregion Set

        #region Refresh

        public virtual void Refresh(TCacheKey key, bool? hideErrors = null)
        {
            hideErrors ??= _distributedCacheOption.HideErrors;

            try
            {
                Cache.Refresh(NormalizeKey(key));
            }
            catch (Exception ex)
            {
                if (hideErrors == true)
                {
                    HandleException(ex);
                    return;
                }

                throw;
            }
        }

        public virtual async Task RefreshAsync(TCacheKey key, bool? hideErrors = null, CancellationToken token = default)
        {
            hideErrors ??= _distributedCacheOption.HideErrors;

            try
            {
                await Cache.RefreshAsync(NormalizeKey(key), token);
            }
            catch (Exception ex)
            {
                if (hideErrors == true)
                {
                    HandleException(ex);

                    return;
                }

                throw;
            }
        }

        #endregion Refresh

        #region Remove

        public virtual void Remove(TCacheKey key, bool? hideErrors = null)
        {
            try
            {
                hideErrors ??= _distributedCacheOption.HideErrors;

                Cache.Remove(NormalizeKey(key));
            }
            catch (Exception ex)
            {
                if (hideErrors == true)
                {
                    HandleException(ex);
                    return;
                }

                throw;
            }
        }

        public virtual async Task RemoveAsync(TCacheKey key, bool? hideErrors = null, CancellationToken token = default)
        {
            try
            {
                hideErrors ??= _distributedCacheOption.HideErrors;

                await Cache.RemoveAsync(NormalizeKey(key), token);
            }
            catch (Exception ex)
            {
                if (hideErrors == true)
                {
                    HandleException(ex);
                    return;
                }

                throw;
            }
        }

        #endregion Remove

        #region Utils

        protected virtual void SetDefaultOptions()
        {
            CacheName = CacheNameAttribute.GetCacheName(typeof(TCacheItem));

            //Configure default cache entry options
            DefaultCacheOptions = GetDefaultCacheEntryOptions();
        }

        protected virtual DistributedCacheEntryOptions GetDefaultCacheEntryOptions()
        {
            foreach (var configure in _distributedCacheOption.CacheConfigurators)
            {
                var options = configure.Invoke(CacheName);

                if (options != null)
                {
                    return options;
                }
            }

            return _distributedCacheOption.GlobalCacheEntryOptions;
        }

        protected virtual string NormalizeKey(TCacheKey key)
        {
            return KeyNormalizer.NormalizeKey(
                new DistributedCacheKeyNormalizeArgs(
                    key.ToString(),
                    CacheName
                )
            );
        }

        [CanBeNull]
        protected virtual TCacheItem ToCacheItem([CanBeNull] byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }

            return Serializer.Deserialize<TCacheItem>(bytes);
        }

        #endregion Utils

        #region Handling Exception

        protected virtual void HandleException(Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }

        #endregion Handling Exception
    }
}