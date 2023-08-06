using System.Diagnostics.CodeAnalysis;
using FinbourneCache.Services.Cache;
using FinbourneCache.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace FinbourneCache.Configuration.Cache;

[ExcludeFromCodeCoverage]
public static class CacheServices
{
    public static void AddCacheServices(this WebApplicationBuilder builder)
    {
	    builder.Services.AddTransient<CacheManagerService>();
	    builder.Services.AddTransient<ICacheService<object, object>, CacheService<object, object>>();
        builder.Services.AddTransient<AuthenticationService>();
    }
}