namespace TwitPoster.BLL.External.Location;

public interface ILocationClient
{
    Task<CountriesResponse> GetCountries(CancellationToken cancellationToken = default);
    Task<StatesResponse> GetStates(string countryName, CancellationToken cancellationToken = default);
    Task<CitiesResponse> GetCities(string countryName, string stateName, CancellationToken cancellationToken = default);
}