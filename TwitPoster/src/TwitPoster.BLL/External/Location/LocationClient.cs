﻿using System.Net.Http.Json;

namespace TwitPoster.BLL.External.Location;

public class LocationClient : ILocationClient
{
    private readonly HttpClient _httpClient;
    
    public LocationClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<CountriesResponse> GetCountries(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("api/v0.1/countries/flag/unicode", cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<CountriesResponse>(cancellationToken);

        return content!;
    }

    public async Task<StatesResponse> GetStates(string countryName, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/v0.1/countries/states", new { country = countryName }, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<StatesResponse>(cancellationToken);

        return content!;
    }

    public async Task<CitiesResponse> GetCities(string countryName, string stateName, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/v0.1/countries/state/cities", new { country = countryName, state = stateName }, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<CitiesResponse>(cancellationToken);

        return content!;
    }
}