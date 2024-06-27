using System.Net;
using Mapster;
using TwitPoster.BLL.DTOs.Location;
using TwitPoster.BLL.External.Location;
using TwitPoster.BLL.Interfaces;

namespace TwitPoster.BLL.Services;

public class LocationService(ILocationClient locationClient, ICacheService cacheService) : ILocationService
{
    public async Task<IReadOnlyList<Country>> GetCountries(CancellationToken cancellationToken = default)
    {
        var countries = await GetFromCacheOrCreate<Country>("countries", async () =>
        {
            var result = await locationClient.GetCountries(cancellationToken);

            return result.Data.Adapt<IReadOnlyList<Country>>().ToList();
        });
        
        return countries;
    }

    public async Task<IReadOnlyList<string>> GetStates(string countryName, CancellationToken cancellationToken = default)
    {
        return await GetFromCacheOrCreate<string>($"country-{countryName}-states", async () =>
        {
            var result = await locationClient.GetStates(new CountriesStatesRequest(countryName), cancellationToken);
            return result.Data.States.Select(x => x.Name).ToList();
        });
    }

    public async Task<IReadOnlyList<string>> GetCities(string countryName, string stateName, CancellationToken cancellationToken = default)
    {
        var cities = await GetFromCacheOrCreate<string>($"country-{countryName}-state-{stateName}-cities", async () => 
            (await locationClient.GetCities(new CountriesCitiesRequest(countryName, stateName), cancellationToken)).Data);

        return cities;
    }
    
    private async Task<IReadOnlyList<T>> GetFromCacheOrCreate<T>(string key, Func<Task<IReadOnlyList<T>>> factory)
    {
        try
        {
            var result = await cacheService.GetFromCacheOrCreate(key, async () => await factory());

            return result!;

        }
        catch (HttpRequestException e) when(e.StatusCode == HttpStatusCode.NotFound)
        {
            return new List<T>();
        }
    }
}