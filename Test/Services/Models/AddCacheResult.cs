using FinbourneCache.Domain.DataTransferObjects.Response;
using FinbourneCache.Services.Enums;

namespace FinbourneCache.Services.Models;

public class AddCacheResult
{
    public AddedToCache AddedToCache { get; set; }
    public CacheItem? ItemRemovedFromCache { get; set; }
}