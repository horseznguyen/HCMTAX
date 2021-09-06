namespace Services.Common.Caching.Interfaces
{
    public interface IDistributedCacheKeyNormalizer
    {
        string NormalizeKey(DistributedCacheKeyNormalizeArgs args);
    }
}