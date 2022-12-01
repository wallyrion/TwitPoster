using TwitPoster.Web.ViewModels.Post;

namespace TwitPoster.Web.ViewModels;

public record PagedResponse(IEnumerable<PostCommentViewModel> Items, int TotalCount);