using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TwitPoster.BLL.DTOs;
using TwitPoster.BLL.Exceptions;
using TwitPoster.BLL.Interfaces;
using TwitPoster.DAL;
using TwitPoster.DAL.Models;
using TwitPoster.Web.Common;
using TwitPoster.Web.Common.Options;
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
    
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateUserProfile(UpdateUserProfileRequest request)
    {
        var command = request.Adapt<UpdateUserProfileCommand>();
        await _usersService.UpdateUserProfile(command);
        return NoContent();
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
    public async Task<ActionResult<UploadPhotoResponse>> UploadPhoto(IFormFile file, [FromServices] BlobServiceClient blobServiceClient, [FromServices] TwitPosterContext context, [FromServices] IOptions<StorageOptions> storageOptions)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!WebConstants.ValidProfileImageExtensions.Contains(extension)){
            return BadRequest("Invalid file type for the image");
        }

        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(storageOptions.Value.ContainerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
        
        var profilePhotosPath = $"user/{_currentUser.Id}/images/profile";
        
        var mainPhotoBlob = containerClient.GetBlobClient($"{profilePhotosPath}/main/image{extension}");
        var thumbnailPhotoBlob = containerClient.GetBlobClient($"{profilePhotosPath}/thumbnail/image{extension}");
        
        await mainPhotoBlob.UploadAsync(file.OpenReadStream(), true);

        var user = await context.Users.FirstAsync(x => x.Id == _currentUser.Id) ?? throw new TwitPosterValidationException($"User {_currentUser.Id} not found");
        user.PhotoUrl = mainPhotoBlob.Uri.ToString();
        user.ThumbnailPhotoUrl = thumbnailPhotoBlob.Uri.ToString();
        await context.SaveChangesAsync();

        return Ok(new UploadPhotoResponse(mainPhotoBlob.Uri.ToString()));
    }
}