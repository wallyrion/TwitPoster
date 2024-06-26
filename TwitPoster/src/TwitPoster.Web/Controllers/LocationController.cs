using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitPoster.BLL.DTOs.Location;
using TwitPoster.BLL.Interfaces;

namespace TwitPoster.Web.Controllers;

[Route("[controller]")]
[ApiController]
[AllowAnonymous]
public class LocationController(ILocationService locationService) : ControllerBase
{
    [HttpGet("countries")]
    public async Task<IReadOnlyList<Country>> GetCountriesAsync(CancellationToken cancellationToken = default)
    {
        return await locationService.GetCountries(cancellationToken);
    }
    
    [HttpGet("{countryName}/states")]
    public async Task<IReadOnlyList<string>> GetStates(string countryName, CancellationToken cancellationToken = default)
    {
        return await locationService.GetStates(countryName, cancellationToken);
    }
    
    [HttpGet("{countryName}/{stateName}/cities")]
    public async Task<IReadOnlyList<string>> GetCountriesAsync(string countryName, string stateName, CancellationToken cancellationToken = default)
    {
        return await locationService.GetCities(countryName, stateName, cancellationToken);
    }
}