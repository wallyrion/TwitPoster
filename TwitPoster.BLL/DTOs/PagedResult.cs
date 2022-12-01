namespace TwitPoster.BLL.DTOs;

public record PagedResult(IEnumerable<PostCommentDto> Items, int TotalCount);
