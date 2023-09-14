using Microsoft.Extensions.Caching.Memory;

namespace TwitPoster.BLL.Services;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    
    public async Task<T?> GetFromCacheOrCreate<T>(string key, Func<Task<T>> factory, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
    {
        expirationTime ??= TimeSpan.FromMinutes(10);
        
        var result = await _memoryCache.GetOrCreateAsync<T>(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = expirationTime;
            return await factory();
        });

        return result!;
    }
}