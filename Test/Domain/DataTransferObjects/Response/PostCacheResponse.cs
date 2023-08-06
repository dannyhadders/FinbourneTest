namespace FinbourneCache.Domain.DataTransferObjects.Response
{
    public class PostCacheResponse :BaseResponse 
    {
        public CacheItem CacheItem { get; set; } 
        public CacheItem ItemRemovedFromCache{ get; set; } 
    }
}