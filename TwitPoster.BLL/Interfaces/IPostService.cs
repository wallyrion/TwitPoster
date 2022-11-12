using TwitPoster.BLL.DTOs;
using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Interfaces;

public interface IPostService
{
    Task<List<PostDto>> GetPosts();
    Task<PostDto> CreatePost(string body);
    Task<PostComment> CreateComment(int postId, string text);
    Task<int> LikePost(int postId);
    Task<int> UnlikePost(int postId);
    Task<IEnumerable<PostCommentDto>> GetComments(int postId, int pageSize, int pageNumber);
}