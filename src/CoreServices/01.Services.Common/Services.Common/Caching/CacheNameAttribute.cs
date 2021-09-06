using JetBrains.Annotations;
using System;
using System.Linq;
using Services.Common.StringUtils;

namespace Services.Common.Caching
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class CacheNameAttribute : Attribute
    {
        public string Name { get; set; }

        public CacheNameAttribute([NotNull] string name)
        {
            Name = name;
        }

        public static string GetCacheName(Type cacheItemType)
        {
            var cacheNameAttribute = cacheItemType
                .GetCustomAttributes(true)
                .OfType<CacheNameAttribute>()
                .FirstOrDefault();

            if (cacheNameAttribute != null)
            {
                return cacheNameAttribute.Name;
            }

            return cacheItemType.FullName.RemovePostFix("CacheItem");
        }
    }
}