using System.ComponentModel.DataAnnotations;

namespace TwitPoster.Web.ViewModels;

public record CreatePostRequest([Required] string Body);