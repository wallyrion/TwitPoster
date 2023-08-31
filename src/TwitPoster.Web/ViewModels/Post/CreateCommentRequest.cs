using System.ComponentModel.DataAnnotations;

namespace TwitPoster.Web.ViewModels.Post;

public record CreateCommentRequest([MaxLength(20000)] string Text);