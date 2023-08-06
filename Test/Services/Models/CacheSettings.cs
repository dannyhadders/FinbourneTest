namespace FinbourneCache.Services.Models;

public class CacheSettings
{
    public int Capacity { get; set; }
    public bool RemoveFromCacheIfLimitReached { get; set; }
}