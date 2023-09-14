using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace TwitPoster.BLL.Services;

public class DistributedCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;

    public DistributedCacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }
    
    public async Task<T?> GetFromCacheOrCreate<T>(string key, Func<Task<T>> factory, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
    {
        expirationTime ??= TimeSpan.FromMinutes(10);
        
        var fromCache = await _distributedCache.GetStringAsync(key, cancellationToken);

        if (fromCache == null)
        {
            var res = await factory();
            
            await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(res), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime
            }, cancellationToken);

            return res;
        }

        var deserialized = JsonSerializer.Deserialize<T>(fromCache);

        return deserialized;
    }
}