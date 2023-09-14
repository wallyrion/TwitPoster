using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using TwitPoster.BLL.Common.Constants;
using TwitPoster.BLL.Interfaces;

namespace TwitPoster.BLL.Services;

public class CacheService(IServiceProvider serviceProvider, IFeatureManager featureManager) : ICacheService
{
    public async Task<T?> GetFromCacheOrCreate<T>(string key, Func<Task<T?>> factory, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
    {
        var useRedis = await featureManager.IsEnabledAsync(FeatureFlags.UseDistributedCache);

        var cacheServiceKey = useRedis ? DependencyInjectionKeys.DistributedCacheService : DependencyInjectionKeys.MemoryService;

        var cacheService = serviceProvider.GetRequiredKeyedService<ICacheService>(cacheServiceKey);
        
        return await cacheService.GetFromCacheOrCreate(key, factory, expirationTime, cancellationToken);
    }
}