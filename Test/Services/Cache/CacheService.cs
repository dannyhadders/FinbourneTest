using FinbourneCache.Domain.DataTransferObjects.Requests;
using FinbourneCache.Domain.DataTransferObjects.Response;
using FinbourneCache.Services.Enums;
using FinbourneCache.Services.Interfaces;
using FinbourneCache.Services.Models;
using Microsoft.Extensions.Options;

namespace FinbourneCache.Services.Cache;

public class CacheService<TKey, TValue> : ICacheService<TKey, TValue>
{
    private static readonly Dictionary<TKey, CacheNode> Cache = new();
    private static CacheNode _head = _head;
    private static CacheNode _tail = _tail;
    private readonly CacheSettings _cacheSettings;
    private static readonly object LockObject = new();

    public CacheService(IOptions<CacheSettings> cacheSettings)
    {
        _cacheSettings = cacheSettings.Value;
    }

    public AddCacheResult AddToCache(TKey key, TValue value)
    {
        lock (LockObject)
        {
            var addCacheResult = new AddCacheResult();
            CacheItem itemRemovedFromCache = null;
            // Get from Cache if exists and move Node to front
            if (Cache.TryGetValue(key, out CacheNode node))
            {
                // Update existing item
                node.Value = value;
                MoveNodeToFront(node);
                addCacheResult.AddedToCache = AddedToCache.ItemRetrievedFromCache;
                return addCacheResult;
            }

            // Add a new item
            if (Cache.Count >= _cacheSettings.Capacity)
            {
                if (_cacheSettings.RemoveFromCacheIfLimitReached)
                {
                    // Evict the LRU item (tail)
                    itemRemovedFromCache = RemoveTail();
                }
                else
                {
                    addCacheResult.AddedToCache = AddedToCache.CacheAtCapacity;
                    return addCacheResult;
                }
            }

            var newNode = new CacheNode(key, value);
            Cache.Add(key, newNode);
            MoveNodeToFront(newNode);
            addCacheResult.AddedToCache = AddedToCache.AddedToCache;
            addCacheResult.ItemRemovedFromCache = itemRemovedFromCache;
            return addCacheResult;
        }
    }

    public Dictionary<TKey, CacheNode> GetCacheValues()
    {
        lock (LockObject)
        {
            return Cache;
        }
    }

    public CacheItem? GetCacheById(TKey key)
    {
        lock (LockObject)
        {
            var exists = Cache.ContainsKey(key);
            if (exists)
            {
                var cacheNode = Cache.GetValueOrDefault(key);
                return new CacheItem
                {
                    Key = cacheNode.Key.ToString(),
                    Value = cacheNode.Value
                };
            }

            return null;
        }
    }

    private void MoveNodeToFront(CacheNode node)
    {
        if (node == _head)
        {
            // Node is already the head (MRU position)
            return;
        }

        if (node.Previous != null)
        {
            // Connect the previous node to the next node
            node.Previous.Next = node.Next;
        }

        if (node.Next != null)
        {
            // Connect the next node to the previous node
            node.Next.Previous = node.Previous;
        }

        if (node == _tail)
        {
            // Update the tail if needed
            _tail = node.Previous;
        }

        // Update node's connections
        node.Previous = null;
        node.Next = _head;

        if (_head != null)
        {
            // Update the previous head's previous reference
            _head.Previous = node;
        }

        _head = node;

        if (_tail == null)
        {
            // If the cache was empty, head and tail are the same
            _tail = _head;
        }
    }

    private CacheItem RemoveTail()
    {
        if (_tail != null)
        {
            var cacheItem = GetCacheById(_tail.Key);
            Cache.Remove(_tail.Key);
            _tail = _tail.Previous;

            if (_tail != null)
            {
                _tail.Next = null;
            }
            else
            {
                // If the cache becomes empty, update head as well
                _head = null;
            }

            return cacheItem;
        }

        return new CacheItem();
    }

    public class CacheNode
    {
        public TKey Key { get; }
        public TValue Value { get; set; }
        public CacheNode Previous { get; set; }
        public CacheNode Next { get; set; }

        public CacheNode(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}