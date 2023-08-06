namespace FinbourneCache.Domain.DataTransferObjects.Response
{
    public class GetCacheResponse :BaseResponse 
    {
        public List<CacheItem> CacheResponses { get; set; } 
    }
}