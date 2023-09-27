
namespace TwitPoster.Web.ViewModels;

public record PagedResponse<T>(IEnumerable<T> Items, int TotalCount);