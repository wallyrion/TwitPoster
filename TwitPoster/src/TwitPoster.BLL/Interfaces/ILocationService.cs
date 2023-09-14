using TwitPoster.BLL.DTOs.Location;

namespace TwitPoster.BLL.Interfaces;

public interface ILocationService
{
    Task<IReadOnlyList<Country>> GetCountries (CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetStates(string countryName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetCities (string countryName, string stateName, CancellationToken cancellationToken = default);
}