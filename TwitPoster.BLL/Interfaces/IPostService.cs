using TwitPoster.BLL.DTOs;

namespace TwitPoster.BLL.Interfaces;

public interface IPostService
{
    Task<List<PostDto>> GetPosts(int pageSize, int pageNumber, CancellationToken cancellationToken = default);
    Task<PostDto> CreatePost(string body);
    Task<PostCommentDto> CreateComment(int postId, string text);
    Task<int> LikePost(int postId);
    Task<int> UnlikePost(int postId);
    Task<PagedResult> GetComments(int postId, int pageSize, int pageNumber);
    List<PostDto> GetPostsSync(int pageSize, int pageNumber);
    Task<int> GetPostsCount();
}