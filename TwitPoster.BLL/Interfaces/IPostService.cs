using TwitPoster.BLL.DTOs;
using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Interfaces;

public interface IPostService
{
    Task<List<PostDto>> GetPosts();
    Task<PostDto> CreatePost(string body);
    Task<List<PostComment>> GetComments(int postId);
    Task<PostComment> CreateComment(int postId, string text);
    Task<int> LikePost(int postId);
}