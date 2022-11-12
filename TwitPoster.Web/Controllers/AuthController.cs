using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitPoster.BLL.DTOs;
using TwitPoster.BLL.Interfaces;
using TwitPoster.DAL.Models;
using TwitPoster.Web.Extensions;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.Web.Controllers;

[Route("[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IUsersService _usersService;
    
    public AuthController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpPost("registration")]
    public async Task<ActionResult<RegistrationResponse>> Register(RegistrationRequest request)
    {
        var registerResponse = await _usersService.Register(request.FirstName, request.LastName, request.BirthDate, request.Email, request.Password);

        return this.ToOk(registerResponse, result => new RegistrationResponse(result.UserId, result.AccessToken));
    }
    
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var loginResponse = await _usersService.Login(request.Email, request.Password);
        return Ok(new RegistrationResponse(loginResponse.UserId, loginResponse.AccessToken));
    }
}