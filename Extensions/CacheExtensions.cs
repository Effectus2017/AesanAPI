using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Api.Models;

namespace Api.Extensions;

public static class CacheExtensions
{
    public static async Task<T> CacheQuery<T>(
        this IMemoryCache cache,
        string key,
        Func<Task<T>> query,
        ILogger logger,
        ApplicationSettings appSettings,
        TimeSpan? expiration = null
    )
    {
        // Si el caché está deshabilitado, ejecutar la consulta directamente
        if (!appSettings.Cache.Enabled)
        {
            logger.LogInformation(
                "Cache deshabilitado, ejecutando consulta directamente para key: {Key}",
                key
            );
            return await query();
        }

        if (cache.TryGetValue(key, out T? cachedResult))
        {
            logger.LogInformation("Cache hit para key: {Key}", key);
            return cachedResult!;
        }

        logger.LogInformation("Cache miss para key: {Key}", key);
        var result = await query();

        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(
            expiration ?? TimeSpan.FromHours(appSettings.Cache.DefaultExpirationHours)
        );

        cache.Set(key, result, cacheEntryOptions);
        return result;
    }
}
