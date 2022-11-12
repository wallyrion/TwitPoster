using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitPoster.BLL.DTOs;
using TwitPoster.BLL.Interfaces;
using TwitPoster.DAL.Models;
using TwitPoster.Web.Mappers;
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
    [AllowAnonymous]
    public async Task<IEnumerable<PostViewModel>> Get()
    {
        var posts = await _postService.GetPosts();
        return posts.Select(p => p.ToViewModel());
    }
    
    [HttpGet("{postId:int}/comments")]
    [AllowAnonymous]
    public async Task<IEnumerable<PostCommentDto>> GetComments(
        int postId,
        [Range(1, 1000)] int pageSize = 5,
        [Range(1, int.MaxValue)] int pageNumber = 1)
    {
        return await _postService.GetComments(postId, pageSize, pageNumber);
    }
    
    [HttpPut("{postId:int}/like")]
    public async Task<int> LikePost(int postId)
    {
        return await _postService.LikePost(postId);
    }
    
    [HttpPut("{postId:int}/unlike")]
    public async Task<int> UnlikePost(int postId)
    {
        return await _postService.UnlikePost(postId);
    }
    
    [HttpPost("{postId:int}/comments")]
    public async Task<PostComment> CreateComment(int postId, CreateCommentRequest request)
    {
        return await _postService.CreateComment(postId, request.Text);
    }
    
    [HttpPost]
    public async Task<PostViewModel> Create(CreatePostRequest request)
    {
        var postDto = await _postService.CreatePost(request.Body);

        _logger.LogInformation("Created post by Author {AuthorName}", postDto.AuthorFirstName + postDto.AuthorLastName);
        
        return postDto.ToViewModel();
    }
    
    
}