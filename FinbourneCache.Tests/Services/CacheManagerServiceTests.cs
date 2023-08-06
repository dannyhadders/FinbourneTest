using FinbourneCache.Domain.DataTransferObjects.Response;
using FinbourneCache.Services.Cache;
using FinbourneCache.Services.Helpers;
using FinbourneCache.Services.Interfaces;
using Moq;
using System.Net;
using FinbourneCache.Services.Enums;
using FinbourneCache.Services.Models;

namespace FinbourneCache.Tests.Services
{
    [TestClass]
    public class CacheManagerServiceTests
    {
        private readonly CacheManagerService _cacheManagerService;
        private readonly Mock<ICacheService<object, object>> _cacheServiceMock;

        public CacheManagerServiceTests()
        {
            _cacheServiceMock = new Mock<ICacheService<object, object>>();
            _cacheManagerService = new CacheManagerService(_cacheServiceMock.Object);
        }

        [TestMethod]
        public void GivenAnId_WhenFindingItemInTheCacheByItsId_ThenReturnItemInCacheByTheId()
        {
            // Arrange
            var id = "123";
            var expectedHttpStatusCode = HttpStatusCode.OK;
            var cacheItem = new CacheItem
            {
                Key = id,
                Value = "value"
            };
            var getCacheResponse = new GetCacheResponse
            {
                CacheResponses = new List<CacheItem> { cacheItem }
            };
            _cacheServiceMock.Setup(e => e.GetCacheById(It.IsAny<object>()))
                .Returns(cacheItem);

            // Act
            var result = _cacheManagerService.GetCacheById(id);

            // Assert
            CollectionAssert.AreEqual(getCacheResponse.CacheResponses, result.Result.CacheResponses);
            Assert.AreEqual(expectedHttpStatusCode, result.Result.HttpStatusCode);
            Assert.IsNull(result.Result.Message);
            _cacheServiceMock.Verify(e => e.GetCacheById(id), Times.Once);
        }

        [TestMethod]
        public void GivenAnId_WhenUnableToFindItemInTheCacheByItsId_ThenReturnNotFoundInCacheResponse()
        {
            // Arrange
            var id = "123";
            var expectedHttpStatusCode = HttpStatusCode.NotFound;
            var expectedCacheResponseCount = 1;

            _cacheServiceMock.Setup(e => e.GetCacheById(It.IsAny<object>()))
                .Returns((CacheItem?)null);

            // Act
            var result = _cacheManagerService.GetCacheById(id);

            // Assert
            Assert.AreEqual(expectedCacheResponseCount, result.Result.CacheResponses.Count);
            Assert.IsNull(result.Result.CacheResponses.FirstOrDefault());
            Assert.AreEqual(expectedHttpStatusCode, result.Result.HttpStatusCode);
            Assert.AreEqual(CacheMessagesConstants.ItemNotFoundInCache, result.Result.Message);
            _cacheServiceMock.Verify(e => e.GetCacheById(id), Times.Once);
        }

        [TestMethod]
        public void GivenARequestToReturnTheCache_WhenCacheHasValues_ThenReturnCacheValues()
        {
            // Arrange
            var expectedHttpStatusCode = HttpStatusCode.OK;
            var cacheResult = new Dictionary<object, CacheService<object, object>.CacheNode>();
            _cacheServiceMock.Setup(e => e.GetCacheValues())
                .Returns(cacheResult);

            // Act
            var result = _cacheManagerService.GetCache();

            // Assert
            Assert.AreEqual(expectedHttpStatusCode, result.Result.HttpStatusCode);
            CollectionAssert.AreEqual(cacheResult, result.Result.CacheResponses);
            _cacheServiceMock.Verify(e => e.GetCacheValues(), Times.Once);
        }
        
        [TestMethod]
        public void GivenACacheValueToMap_WhenMappingCacheValuesToListOfCacheItems_ThenReturnMappedValues()
        {
            // Arrange
            object key = "1";
            object value = "value";
            
            var cacheResult = new Dictionary<object, CacheService<object, object>.CacheNode>();
            cacheResult.Add(key, new CacheService<object, object>.CacheNode(key, value));

            // Act
            var result = _cacheManagerService.MapCacheResponse(cacheResult);

            // Assert
            Assert.AreEqual(cacheResult.Count, result.Count);
            Assert.AreEqual(cacheResult.First().Key, result.First().Key);
            Assert.AreEqual(cacheResult.First().Value.Value, result.First().Value);
        }
        
        [TestMethod]
        public void GivenAItemToAddToCache_WhenAddingItemToCache_ThenAddItemToCacheSuccessfully()
        {
            // Arrange
            string key = "1";
            string value = "value";
            var expectedHttpStatusCode = HttpStatusCode.OK;
            var addCacheResult = new AddCacheResult
            {
                AddedToCache = AddedToCache.AddedToCache,
                ItemRemovedFromCache = null
            };

            _cacheServiceMock.Setup(e => e.AddToCache(key, value)).Returns(addCacheResult);

            // Act
            var result = _cacheManagerService.AddToCache(key,value);

            // Assert
            Assert.AreEqual(addCacheResult.ItemRemovedFromCache, result.Result.ItemRemovedFromCache);
            Assert.AreEqual(expectedHttpStatusCode, result.Result.HttpStatusCode);
            Assert.AreEqual(CacheMessagesConstants.ItemAddedToCache, result.Result.Message);
            _cacheServiceMock.Verify(e => e.AddToCache(key, value), Times.Once);
        }
        
        [TestMethod]
        public void GivenAItemToAddToCache_WhenItemRetrievedFromCache_ThenRetrieveItemFromCache()
        {
            // Arrange
            string key = "1";
            string value = "value";
            var expectedHttpStatusCode = HttpStatusCode.OK;
            var addCacheResult = new AddCacheResult
            {
                AddedToCache = AddedToCache.ItemRetrievedFromCache,
                ItemRemovedFromCache = null
            };

            _cacheServiceMock.Setup(e => e.AddToCache(key, value)).Returns(addCacheResult);

            // Act
            var result = _cacheManagerService.AddToCache(key,value);

            // Assert
            Assert.AreEqual(addCacheResult.ItemRemovedFromCache, result.Result.ItemRemovedFromCache);
            Assert.AreEqual(expectedHttpStatusCode, result.Result.HttpStatusCode);
            Assert.AreEqual(CacheMessagesConstants.RetrievedFromCache, result.Result.Message);
            _cacheServiceMock.Verify(e => e.AddToCache(key, value), Times.Once);
        }
        
        [TestMethod]
        public void GivenAItemToAddToCache_WhenCacheAtCapacity_ThenDoNotAddToCache()
        {
            // Arrange
            string key = "1";
            string value = "value";
            var expectedHttpStatusCode = HttpStatusCode.Conflict;
            var addCacheResult = new AddCacheResult
            {
                AddedToCache = AddedToCache.CacheAtCapacity,
                ItemRemovedFromCache = null
            };

            _cacheServiceMock.Setup(e => e.AddToCache(key, value)).Returns(addCacheResult);

            // Act
            var result = _cacheManagerService.AddToCache(key,value);

            // Assert
            Assert.AreEqual(addCacheResult.ItemRemovedFromCache, result.Result.ItemRemovedFromCache);
            Assert.AreEqual(expectedHttpStatusCode, result.Result.HttpStatusCode);
            Assert.AreEqual(CacheMessagesConstants.CacheIsAtCapacity, result.Result.Message);
            _cacheServiceMock.Verify(e => e.AddToCache(key, value), Times.Once);
        }
    }
}