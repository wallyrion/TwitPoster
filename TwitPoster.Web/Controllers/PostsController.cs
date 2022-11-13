using System.ComponentModel.DataAnnotations;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
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
   // [OutputCache(Duration = 300)]
    public async Task<IEnumerable<PostViewModel>> Get(
        [Range(1, 1000)] int pageSize = 25,
        [Range(1, int.MaxValue)] int pageNumber = 1)
    {
        var posts = await _postService.GetPosts(pageSize, pageNumber);
        return posts.Adapt<IEnumerable<PostViewModel>>();
    }
    
    // Get total count of posts
    [HttpGet("count")]
    [AllowAnonymous]
    public async Task<int> GetPostsCount()
    {
        return await _postService.GetPostsCount();
    }
   
    [HttpGet("sync")]
    [AllowAnonymous]
    public IEnumerable<PostViewModel> GetSync(
        [Range(1, 1000)] int pageSize = 25,
        [Range(1, int.MaxValue)] int pageNumber = 1)
    {
        var posts = _postService.GetPostsSync(pageSize, pageNumber);
        return posts.Adapt<IEnumerable<PostViewModel>>();
    }
    
    [HttpGet("{postId:int}/comments")]
    [AllowAnonymous]
    public async Task<IEnumerable<PostCommentViewModel>> GetComments(
        int postId,
        [Range(1, 1000)] int pageSize = 5,
        [Range(1, int.MaxValue)] int pageNumber = 1)
    {
        var comments = await _postService.GetComments(postId, pageSize, pageNumber);
        return comments.Adapt<IEnumerable<PostCommentViewModel>>();
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
    public async Task<PostCommentViewModel> CreateComment(int postId, CreateCommentRequest request)
    {
        var newComment = await _postService.CreateComment(postId, request.Text);
        return newComment.Adapt<PostCommentViewModel>();
    }
    
    [HttpPost]
    public async Task<PostViewModel> Create(CreatePostRequest request)
    {
        var postDto = await _postService.CreatePost(request.Body);

        _logger.LogInformation("Created post by Author {AuthorName}", postDto.AuthorFirstName + postDto.AuthorLastName);
        
        return postDto.Adapt<PostViewModel>();
    }
}