using Refit;

namespace TwitPoster.BLL.External.Location;

public interface ILocationClient
{
    [Get("/api/v0.1/countries/flag/unicode")]
    Task<CountriesResponse> GetCountries(CancellationToken cancellationToken = default);
    
    [Post("/api/v0.1/countries/states")]
    Task<StatesResponse> GetStates([Body] CountriesStatesRequest request, CancellationToken cancellationToken = default);
    
    [Post("/api/v0.1/countries/state/cities")]
    Task<CitiesResponse> GetCities([Body] CountriesCitiesRequest request, CancellationToken cancellationToken = default);
}