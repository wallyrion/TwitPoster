using System.ComponentModel.DataAnnotations;
using System.Net;
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
    public async Task<ActionResult> Register(RegistrationRequest request)
    {
        var registerResponse = await _usersService.Register(request.FirstName, request.LastName, request.BirthDate, request.Email, request.Password);

        return this.ToActionResult(registerResponse, _ => Content("Registration successful. Please, check your email to confirm your account."));
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginRequest request)
    {
        var accessToken = await _usersService.Login(request.Email, request.Password);
        return Ok(accessToken);
    }
    
    [HttpGet("EmailConfirmation")]
    public async Task<ActionResult> EmailConfirmation([Required] Guid token)
    {
        await _usersService.ConfirmEmail(token);
        return Content("Email confirmed successfully");
    }
}