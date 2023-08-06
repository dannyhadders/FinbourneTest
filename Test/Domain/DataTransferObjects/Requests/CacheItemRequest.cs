namespace FinbourneCache.Domain.DataTransferObjects.Requests
{
    public class CacheItemRequest
    {
        public string Id { get; set; }

        public object Data { get; set; }
    }
}