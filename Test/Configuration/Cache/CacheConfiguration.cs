using System.Diagnostics.CodeAnalysis;
using FinbourneCache.Services.Models;

namespace FinbourneCache.Configuration.Cache;

[ExcludeFromCodeCoverage]
public static class CacheConfiguration
{
    public static void AddCacheConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection("CacheSettings"));
    }
}