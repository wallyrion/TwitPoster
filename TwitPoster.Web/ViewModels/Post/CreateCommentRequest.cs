using System.ComponentModel.DataAnnotations;

namespace TwitPoster.Web.ViewModels.Post;

public record CreateCommentRequest([MaxLength(1000)] string Text);