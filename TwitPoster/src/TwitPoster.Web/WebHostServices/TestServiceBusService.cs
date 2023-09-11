using System.Text;
using Microsoft.Extensions.Caching.Distributed;

namespace TwitPoster.Web.WebHostServices;

internal sealed class TestServiceBusService : BackgroundService
{
    private readonly ILogger<TestServiceBusService> _logger;
    private readonly IDistributedCache _cache;

    public TestServiceBusService(ILogger<TestServiceBusService> logger, IDistributedCache cache)
    {
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
