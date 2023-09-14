using System.Net;
using System.Text.Json;
using Mapster;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using TwitPoster.BLL.DTOs.Location;
using TwitPoster.BLL.External.Location;
using TwitPoster.BLL.Interfaces;

namespace TwitPoster.BLL.Services;

public class LocationService : ILocationService
{
    private readonly ILocationClient _locationClient;
    private readonly ICacheService _cacheService;

    public LocationService(ILocationClient locationClient, ICacheService cacheService)
    {
        _locationClient = locationClient;
        _cacheService = cacheService;
    }

    public async Task<IReadOnlyList<Country>> GetCountries(CancellationToken cancellationToken = default)
    {
        var res = await GetFromCacheOrCreate<Country>("countries", async () =>
        {
            var result = await _locationClient.GetCountries(cancellationToken);

            return result.Data.Adapt<IReadOnlyList<Country>>().ToList();
        });
        
        return res;
    }

    public async Task<IReadOnlyList<string>> GetStates(string countryName, CancellationToken cancellationToken = default)
    {
        return await GetFromCacheOrCreate<string>($"country-{countryName}-states", async () =>
        {
            var result = await _locationClient.GetStates(countryName, cancellationToken);
            return result.Data.States.Select(x => x.Name).ToList();
        });
    }

    public async Task<IReadOnlyList<string>> GetCities(string countryName, string stateName, CancellationToken cancellationToken = default)
    {
        var cities = await GetFromCacheOrCreate<string>($"country-{countryName}-state-{stateName}-cities", async () => 
            (await _locationClient.GetCities(countryName, stateName, cancellationToken)).Data);

        return cities;
    }
    
    private async Task<IReadOnlyList<T>> GetFromCacheOrCreate<T>(string key, Func<Task<IReadOnlyList<T>>> factory)
    {
        try
        {
            var result = await _cacheService.GetFromCacheOrCreate<IReadOnlyList<T>>(key, async () =>
            {
                
                return await factory();

            });

            return result!;

        }
        catch (HttpRequestException e) when(e.StatusCode == HttpStatusCode.NotFound)
        {
            return new List<T>();
        }
    }
}

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



public interface ICacheService
{
    public Task<T?> GetFromCacheOrCreate<T>(string key, Func<Task<T>> factory, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default);
}