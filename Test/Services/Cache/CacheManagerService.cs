using System.Net;
using FinbourneCache.Domain.DataTransferObjects.Response;
using FinbourneCache.Services.Enums;
using FinbourneCache.Services.Helpers;
using FinbourneCache.Services.Interfaces;

namespace FinbourneCache.Services.Cache;

public class CacheManagerService
{
    private readonly ICacheService<object, object> _cacheService;

    public CacheManagerService(ICacheService<object, object> cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<PostCacheResponse> AddToCache(string id, object value)
    {
        var addCacheResult = _cacheService.AddToCache(id, value);
        if (addCacheResult.AddedToCache == AddedToCache.CacheAtCapacity)
        {
            return new PostCacheResponse
            {
                HttpStatusCode = HttpStatusCode.Conflict,
                Message = CacheMessagesConstants.CacheIsAtCapacity,
                CacheItem = null,
                ItemRemovedFromCache = null
            };
        }

        var cacheResponse = new CacheItem
        {
            Key = id,
            Value = value
        };


        if (addCacheResult.AddedToCache == AddedToCache.ItemRetrievedFromCache)
        {
            return new PostCacheResponse
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = CacheMessagesConstants.RetrievedFromCache,
                CacheItem = cacheResponse,
                ItemRemovedFromCache = null
            };
        }
        

        return new PostCacheResponse
        {
            HttpStatusCode = HttpStatusCode.OK,
            Message = CacheMessagesConstants.ItemAddedToCache,
            CacheItem = cacheResponse,
            ItemRemovedFromCache = addCacheResult.ItemRemovedFromCache
        };
    }
    
    public async Task<GetCacheResponse> GetCache()
    {
        var cacheValues = _cacheService.GetCacheValues();
        var cacheResponses = MapCacheResponse(cacheValues);
        var getCacheResponse = new GetCacheResponse
        {
            CacheResponses = cacheResponses,
            HttpStatusCode = HttpStatusCode.OK
        };

        return getCacheResponse;

    }
    
    public async Task<GetCacheResponse> GetCacheById(object id)
    {
        var cacheResponse = _cacheService.GetCacheById(id);
        var getCacheResponse = new GetCacheResponse
        {
            CacheResponses = new List<CacheItem>{cacheResponse}
        };

        if (cacheResponse == null)
        {
            getCacheResponse.HttpStatusCode = HttpStatusCode.NotFound;
            getCacheResponse.Message = CacheMessagesConstants.ItemNotFoundInCache;
            return getCacheResponse;
        }

        getCacheResponse.HttpStatusCode = HttpStatusCode.OK;
        return getCacheResponse;

    }

    public List<CacheItem> MapCacheResponse(Dictionary<object, CacheService<object, object>.CacheNode> cacheValues)
    {
        var cacheResponses = new List<CacheItem>();
        
        foreach (var cacheValue in cacheValues)
        {
            var cacheResponse = new CacheItem
            {
                Key = cacheValue.Key.ToString(),
                Value = cacheValue.Value.Value
            };
            cacheResponses.Add(cacheResponse);
        }

        return cacheResponses;
    }
}