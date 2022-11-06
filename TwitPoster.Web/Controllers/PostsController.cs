using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitPoster.BLL.Interfaces;
using TwitPoster.DAL;
using TwitPoster.DAL.Models;
using TwitPoster.Web.Extensions;
using TwitPoster.Web.Mappers;
using TwitPoster.Web.Middlewares;
using TwitPoster.Web.ViewModels;
using TwitPoster.Web.ViewModels.Post;

namespace TwitPoster.Web.Controllers;

[Route("[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly ILogger<PostsController> _logger;
    private readonly IPostService _postService;
    
    public PostsController(ILogger<PostsController> logger, IPostService postService)
    {
        _logger = logger;
        _postService = postService;
    }

    [HttpGet]
    public async Task<IEnumerable<PostViewModel>> Get()
    {
        var posts = await _postService.GetPosts();
        return posts.Select(p => p.ToViewModel());
    }
    
    [HttpGet("{postId:int}/comments")]
    public async Task<IEnumerable<PostComment>> GetComments(int postId)
    {
        return await _postService.GetComments(postId);
    }
    
    [HttpPost("{postId:int}/comments")]
    [Authorize]
    public async Task<PostComment> CreateComment(int postId, CreateCommentRequest request)
    {
        return await _postService.CreateComment(postId, request.Text);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<PostViewModel> Create(CreatePostRequest request)
    {
        var postDto = await _postService.CreatePost(request.Body);

        _logger.LogInformation("Created post by Author {AuthorName}", postDto.AuthorFirstName + postDto.AuthorLastName);
        
        return postDto.ToViewModel();
    }
    
    
}