using TwitPoster.BLL.DTOs;
using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Interfaces;

public interface IPostService
{
    Task<List<PostDto>> GetPosts(int pageSize, int pageNumber);
    Task<PostDto> CreatePost(string body);
    Task<PostCommentDto> CreateComment(int postId, string text);
    Task<int> LikePost(int postId);
    Task<int> UnlikePost(int postId);
    Task<IEnumerable<PostCommentDto>> GetComments(int postId, int pageSize, int pageNumber);
    List<PostDto> GetPostsSync(int pageSize, int pageNumber);
    Task<int> GetPostsCount();
}