using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using TwitPoster.BLL.Interfaces;
using TwitPoster.Web.Extensions;
using TwitPoster.Web.SwaggerExamples.User;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.Web.Controllers;

[Route("[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;
    
    public UsersController(IUsersService usersService)
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