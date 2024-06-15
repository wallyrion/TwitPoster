using System.Threading.RateLimiting;
using TwitPoster.BLL.Common.Constants;
using TwitPoster.BLL.Common.Options;
using TwitPoster.BLL.Extensions;
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
    
    public static IServiceCollection AddTwitPosterRateLimiting(this IServiceCollection services, IConfigurationManager configuration)
    {
        var features = configuration.BindOption<FeatureFlagsOptions>(services, false);

        if (!features.UseRateLimiting)
        {
            return services;
        }
        
        services.AddRateLimiter(limiterOptions =>
        {
            limiterOptions.GlobalLimiter =
                PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var clientIP = httpContext.Connection.RemoteIpAddress?.ToString();

                    return RateLimitPartition.GetFixedWindowLimiter(
                        clientIP ?? "static", _ => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 5,
                            Window = TimeSpan.FromSeconds(30)
                        });
                });
        });

        return services;
    } 
}