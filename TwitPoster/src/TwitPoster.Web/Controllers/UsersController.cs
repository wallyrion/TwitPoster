using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
    public async Task<ActionResult> UploadPhoto(IFormFile file, [FromServices] BlobServiceClient blobServiceClient)
    {
        if (!Request.Form.Files.Any())
        {
            return BadRequest("At least one file must be uploaded");
        }
        
        var formFile = Request.Form.Files[0];
        
        var directoryPath = Path.Combine("uploadImages", _currentUser.Id.ToString(), "profile", formFile.FileName );
        
        BlobContainerClient containerClient = 
            blobServiceClient.GetBlobContainerClient("twitposter2");

        await containerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);

        var blob = containerClient.GetBlobClient(directoryPath);
        var response = await blob.UploadAsync(formFile.OpenReadStream(), true);

        var blobNew = blob.Uri;
        
        return NoContent();
    }
    
    [HttpPost("photoOld")]
    public async Task<ActionResult> UploadPhotoOld(IFormFile file)
    {
        if (!Request.Form.Files.Any())
        {
            return BadRequest("At least one file must be uploaded");
        }
        
        var formFile = Request.Form.Files[0];
        
        var directoryPath = Path.Combine("uploadImages", _currentUser.Id.ToString());
        
        Directory.CreateDirectory(directoryPath);

        foreach (var exisingFile in Directory.GetFiles(directoryPath))
        {
            System.IO.File.Delete(exisingFile);
        }
        var path = Path.Combine(directoryPath, formFile.FileName);

        await using var stream = System.IO.File.OpenWrite(path);
        await file.CopyToAsync(stream);

        return NoContent();
    }
    
    [AllowAnonymous]
    [HttpGet("{userId:int}/photo")]
    public async Task<ActionResult> GetProfilePhoto(int userId,  [FromServices] BlobServiceClient blobServiceClient)
    {
        var result = await GetPhoto(userId);
        if (result == null)
        {
            return NoContent();
        }
        
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(result.Value.fileName, out var contentType))
        {
            contentType = System.Net.Mime.MediaTypeNames.Application.Octet;
        }
        
        return File(result.Value.file, contentType, result.Value.fileName);
    }

    
    [AllowAnonymous]
    [HttpGet("{userId:int}/photoold")]
    public async Task<ActionResult> GetProfilePhotoold(int userId)
    {
        var result = await GetPhoto(userId);
        if (result == null)
        {
            return NoContent();
        }
        
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(result.Value.fileName, out var contentType))
        {
            contentType = System.Net.Mime.MediaTypeNames.Application.Octet;
        }
        
        return File(result.Value.file, contentType, result.Value.fileName);
    }
    
    private async Task<(byte[] file, string fileName)?> GetPhoto(int userId)
    {
        var directoryPath = Path.Combine("uploadImages", userId.ToString());

        if (!Directory.Exists(directoryPath))
        {
            return null;
        }
        
        var filePath = Directory.GetFiles(directoryPath).FirstOrDefault();
        
        if (filePath is null)
        {
            return null;
        }
        
        var fileName = Path.GetFileName(filePath);
        var fileContent = await System.IO.File.ReadAllBytesAsync(filePath);
        return (fileContent, fileName);
    }
}