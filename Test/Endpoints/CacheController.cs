using FinbourneCache.Domain.DataTransferObjects.Requests;
using FinbourneCache.Services.Cache;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FinbourneCache.Endpoints
{
    public static class CacheController
    {
        public static void MapCacheEndPoints(this WebApplication app)
        {
            app.MapPost("/cache", [SwaggerOperation(Summary = "cache request", Description = "adds an object to the cache")]
            [SwaggerResponse(200, "Success")]
            [SwaggerResponse(401, "Authentication failure")]
            [SwaggerResponse(500, "Internal server error with given error message")]
            //[Authorize(AuthenticationSchemes = $"{ApiKeyAuthenticationOptions.DefaultScheme}")]
            async (CacheItemRequest request, CacheManagerService cacheManagerService, AuthenticationService authenticationService, HttpContext context) =>
                {
                    var cacheResult = await cacheManagerService.AddToCache(request.Id, request.Data);
                    if (cacheResult.IsHttpStatusCodeOk)
                    {
                        return Results.Ok(cacheResult);
                    }
                    return Results.Problem(cacheResult.Message, statusCode: (int)cacheResult.HttpStatusCode);
                });

            app.MapGet("/cache", [SwaggerOperation(Summary = "cache request", Description = "gets all objects in the cache")]
            [SwaggerResponse(200, "Success")]
            [SwaggerResponse(401, "Authentication failure")]
            [SwaggerResponse(500, "Internal server error with given error message")]
            //[Authorize(AuthenticationSchemes = $"{ApiKeyAuthenticationOptions.DefaultScheme}")]
            async (CacheManagerService cacheManagerService, AuthenticationService authenticationService, HttpContext context) =>
                {

                    var cacheResult = await cacheManagerService.GetCache();
                    return Results.Ok(cacheResult);

                });

            app.MapGet("/cache/{id}", [SwaggerOperation(Summary = "cache request", Description = "gets object to the cache by its id")]
            [SwaggerResponse(200, "Success")]
            [SwaggerResponse(401, "Authentication failure")]
            [SwaggerResponse(500, "Internal server error with given error message")]
            //[Authorize(AuthenticationSchemes = $"{ApiKeyAuthenticationOptions.DefaultScheme}")]
            async ([FromRoute] string id, CacheManagerService cacheManagerService, AuthenticationService authenticationService, HttpContext context) =>
                {
                    var cacheResult = await cacheManagerService.GetCacheById(id);
                    return Results.Ok(cacheResult);
                });
        }
    }
}