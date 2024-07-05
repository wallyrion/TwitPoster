using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TwitPoster.BLL.Common.Options;
using TwitPoster.BLL.Interfaces;
using TwitPoster.Web.Common.Auth;
using TwitPoster.Web.Extensions;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.Web.Controllers;

[Route("[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("google-sso")]
    public async Task<ActionResult> LoginWithGoogle([FromForm] GoogleRequest request, [FromQuery] string redirectTo, IOptions<ApplicationOptions> applicationOptions)
    {
        var payload = await GoogleSignInHelper.ValidateGoogleToken(request.Credential, applicationOptions.Value.GoogleClientId);
        if (payload == null)
        {
            return Unauthorized("Invalid Google token");
        }
        
        var token = await authService.LoginWithGoogle(payload.Email, payload.GivenName, payload.FamilyName, payload.EmailVerified, payload.Picture);

        var redirectUrl = $"{redirectTo}?token={token}";

        return Redirect(redirectUrl);
    }
    
    
    [HttpPost("login-google")]
    public async Task<ActionResult> LoginWithGoogleNew([FromBody] GoogleRequest request, IOptions<ApplicationOptions> applicationOptions)
    {
        var payload = await GoogleSignInHelper.ValidateGoogleToken(request.Credential, applicationOptions.Value.GoogleClientId);
        if (payload == null)
        {
            return Unauthorized("Invalid Google token");
        }
        
        var accessToken = await authService.LoginWithGoogle(payload.Email, payload.GivenName, payload.FamilyName, payload.EmailVerified, payload.Picture);

        return Ok(new LoginResponse(accessToken));
    }
    
    [HttpPost("registration")]
    public async Task<ActionResult> Register(RegistrationRequest request)
    {
        var registerResponse = await authService.Register(request.FirstName, request.LastName, request.BirthDate, request.Email, request.Password);

        return this.ToActionResult(registerResponse, _ => Content("Registration successful. Please, check your email to confirm your account."));
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginRequest request)
    {
        var accessToken = await authService.Login(request.Email, request.Password);

        return Ok(new LoginResponse(accessToken));
    }

    [HttpGet("EmailConfirmation")]
    public async Task<ActionResult> EmailConfirmation([Required] Guid token)
    {
        await authService.ConfirmEmail(token);

        return Content("Email confirmed successfully");
    }
}