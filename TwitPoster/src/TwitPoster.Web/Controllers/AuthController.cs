using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitPoster.BLL.Interfaces;
using TwitPoster.Web.Extensions;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.Web.Controllers;

[Route("[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("registration")]
    public async Task<ActionResult> Register(RegistrationRequest request)
    {
        var registerResponse = await _authService.Register(request.FirstName, request.LastName, request.BirthDate, request.Email, request.Password);

        return this.ToActionResult(registerResponse, _ => Content("Registration successful. Please, check your email to confirm your account."));
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginRequest request)
    {
        var accessToken = await _authService.Login(request.Email, request.Password);

        return Ok(new LoginResponse(accessToken));
    }

    [HttpGet("EmailConfirmation")]
    public async Task<ActionResult> EmailConfirmation([Required] Guid token)
    {
        await _authService.ConfirmEmail(token);

        return Content("Email confirmed successfully");
    }
}