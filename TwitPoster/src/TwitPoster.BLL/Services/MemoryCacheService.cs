using Microsoft.Extensions.Caching.Memory;
using TwitPoster.BLL.Interfaces;

namespace TwitPoster.BLL.Services;

public class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
{
    public async Task<T?> GetFromCacheOrCreate<T>(string key, Func<Task<T?>> factory, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
    {
        expirationTime ??= TimeSpan.FromMinutes(10);

        var fromCache = memoryCache.Get<T>(key);

        if (fromCache is not null)
        {
            return fromCache;
        }

        var value = await factory();

        if (value is null)
        {
            return default;
        }

        memoryCache.Set(key, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime
        });

        return value;
    }
}