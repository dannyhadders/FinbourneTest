using FinbourneCache.Domain.DataTransferObjects.Response;
using FinbourneCache.Services.Cache;
using FinbourneCache.Services.Models;

namespace FinbourneCache.Services.Interfaces;

public interface ICacheService<TKey, TValue>
{
    AddCacheResult AddToCache(TKey key, TValue value);
    Dictionary<TKey, CacheService<TKey, TValue>.CacheNode> GetCacheValues();
    CacheItem? GetCacheById(TKey key);
}