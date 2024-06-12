using System.Configuration;
using AspNetCoreRateLimit;
using TwitPoster.BLL.Common.Constants;
using TwitPoster.BLL.Interfaces;
using TwitPoster.BLL.Services;

namespace TwitPoster.Web.Common.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddTwitPosterCaching(this IServiceCollection services, IConfigurationManager configuration)
    {
        services
            .AddMemoryCache();
            
        services
            .AddStackExchangeRedisCache(x =>
            {
                x.Configuration = configuration.GetConnectionString("Redis")!;
            })
                
            .AddScoped<ICacheService, CacheService>()
            .AddKeyedScoped<ICacheService, DistributedCacheService>(DependencyInjectionKeys.DistributedCacheService)
            .AddKeyedScoped<ICacheService, MemoryCacheService>(DependencyInjectionKeys.MemoryService);

        return services;
    } 
    
    public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddInMemoryRateLimiting();
        
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        
        services.Configure<IpRateLimitOptions>(c =>
        {
            c.GeneralRules =
            [
                new RateLimitRule
                {
                    Endpoint = "*",
                    Limit = 1000,
                    Period = "1m"
                }
            ];
        });
        
        return services;
    } 
}