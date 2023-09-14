using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using TwitPoster.BLL.Interfaces;

namespace TwitPoster.BLL.Services;

public class DistributedCacheService(IDistributedCache distributedCache) : ICacheService
{
    public async Task<T?> GetFromCacheOrCreate<T>(string key, Func<Task<T?>> factory, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
    {
        var fromCache = await distributedCache.GetStringAsync(key, cancellationToken);

        if (fromCache is not null)
        {
            return JsonSerializer.Deserialize<T>(fromCache);
        }

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };
            
        var response = await factory();
            
        if (response is null)
        {
            return default;
        }

        var serialized = JsonSerializer.Serialize(response);
        await distributedCache.SetStringAsync(key, serialized, options, cancellationToken);

        return response;
    }
}