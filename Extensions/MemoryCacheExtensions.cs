using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Api.Extensions;

public static class MemoryCacheExtensions
{
    private static readonly Func<MemoryCache, object> GetEntriesCollection;

    static MemoryCacheExtensions()
    {
        // Obtener el campo _entries a través de reflexión
        var entriesField = typeof(MemoryCache).GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance);
        if (entriesField == null)
        {
            throw new Exception("No se pudo encontrar el campo _entries en MemoryCache");
        }

        GetEntriesCollection = cache => entriesField.GetValue(cache);
    }

    public static IEnumerable<string> GetKeys<T>(this IMemoryCache cache)
    {
        if (cache is not MemoryCache memoryCache)
        {
            return Enumerable.Empty<string>();
        }

        var entriesCollection = GetEntriesCollection(memoryCache);
        if (entriesCollection is not ConcurrentDictionary<object, ICacheEntry> entries)
        {
            return Enumerable.Empty<string>();
        }

        return entries.Keys.Select(k => k.ToString()!).Where(k => k != null);
    }

    public static void RemoveByPattern(this IMemoryCache cache, string pattern, ILogger logger = null)
    {
        var allKeys = cache.GetKeys<string>().ToList();
        logger?.LogInformation($"Total keys in cache: {allKeys.Count}");
        logger?.LogInformation($"Current keys in cache: {string.Join(", ", allKeys)}");
        logger?.LogInformation($"Buscando claves con patrón: {pattern}");

        var keys = allKeys.Where(k => k.Contains(pattern)).ToList();
        logger?.LogInformation($"Claves encontradas para eliminar: {string.Join(", ", keys)}");

        foreach (var key in keys)
        {
            cache.Remove(key);
            logger?.LogInformation($"Clave eliminada: {key}");
        }
    }
}