using TwitPoster.BLL.Services;
using TwitPoster.Web.Common.Options;

namespace TwitPoster.Web.Common.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddTwitPosterCaching(this IServiceCollection services, IConfigurationManager configuration)
    {
        var featuresConfig = configuration.GetRequiredSection("Features");
        
        var featuresOptions = featuresConfig.Get<FeatureOptions>()!;

        if (featuresOptions.UseRedisCache)
        {
            services
                .AddStackExchangeRedisCache(x =>
                {
                    x.Configuration = configuration.GetConnectionString("Redis")!;
                })
                .AddScoped<ICacheService, DistributedCacheService>();
        }
        else
        {
            services
                .AddMemoryCache()
                .AddScoped<ICacheService, MemoryCacheService>();
        }

        return services;
    } 
}