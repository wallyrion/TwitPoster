using System.Text;
using Microsoft.Extensions.Caching.Distributed;

namespace TwitPoster.Web.WebHostServices;

internal sealed class TestServiceBusService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TestServiceBusService> _logger;
    private readonly IDistributedCache _cache;

    public TestServiceBusService(IServiceProvider serviceProvider, ILogger<TestServiceBusService> logger, IDistributedCache cache)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _cache.SetStringAsync("test", "test", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        }, stoppingToken);
        
        var res = await _cache.GetStringAsync("test", stoppingToken);
        
        _logger.LogInformation("Test cache: {ResStringFromCache}", res);
    }
}
