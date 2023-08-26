using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using TwitPoster.BLL.DTOs;
using TwitPoster.BLL.Interfaces;
using TwitPoster.DAL.Models;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.Web.Controllers;

[Route("[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;
    private readonly ICurrentUser _currentUser;
    public UsersController(IUsersService usersService, ICurrentUser currentUser)
    {
        _usersService = usersService;
        _currentUser = currentUser;
    }

    [HttpGet("me")]
    public async Task<ActionResult<AccountDetailDto>> GetCurrentUser()
    {
        return await _usersService.GetCurrentAccountDetail();
    }

    [HttpPut("ban/{userId:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task Ban(int userId)
    {
        await _usersService.Ban(userId);
    }

    [HttpPut("unsubscribe/{userId:int}")]
    public void Unsubscribe(int userId)
    {
        _usersService.UnsubscribeAsync(userId);
    }

    [HttpGet("subscriptions")]
    public async Task<List<UserSubscriptionViewModel>> GetSubscriptions()
    {
        var subscriptions = await _usersService.GetSubscriptions();

        return subscriptions.Adapt<List<UserSubscriptionViewModel>>();
    }

    [HttpPut("subscribe/{userId:int}")]
    public async Task Subscribe(int userId)
    {
        var id = userId;

        await _usersService.Subscribe(id);
    }

    [HttpGet("subscribers")]
    public async Task<List<UserSubscriptionViewModel>> GetSubscribers()
    {
        var subscriptions = await _usersService.GetSubscribers();

        return subscriptions.Adapt<List<UserSubscriptionViewModel>>();
    }
    
    [HttpPost("photo")]
    public async Task<ActionResult> UploadPhoto(IFormFile file)
    {
        if (!Request.Form.Files.Any())
        {
            return BadRequest("At least one file must be uploaded");
        }
        
        var formFile = Request.Form.Files[0];
        
        var directoryPath = Path.Combine("uploadImages", _currentUser.Id.ToString());
        
        Directory.CreateDirectory(directoryPath);
        var path = Path.Combine(directoryPath, formFile.FileName);

        await using var stream = System.IO.File.OpenWrite(path);
        await file.CopyToAsync(stream);

        return Ok();
    }
    
    [HttpGet("photo")]
    public async Task<ActionResult> GetProfilePhoto()
    {
        var result = await GetPhoto();
        if (result == null)
        {
            return NoContent();
        }
        
        var provider = new FileExtensionContentTypeProvider();
        
        if (!provider.TryGetContentType(result.Value.fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        
        return File(result.Value.file, contentType, result.Value.fileName);
    }
    
    private async Task<(byte[] file, string fileName)?> GetPhoto()
    {
        var directoryPath = Path.Combine("uploadImages", _currentUser.Id.ToString());

        if (!Directory.Exists(directoryPath))
        {
            return null;
        }
        
        var files = Directory.GetFiles(directoryPath);
        
        if (files.Length == 0)
        {
            return null;
        }
        
        var path = files[0];

        var fileName = Path.GetFileName(path);

        return (await System.IO.File.ReadAllBytesAsync(path), fileName);
    }
}