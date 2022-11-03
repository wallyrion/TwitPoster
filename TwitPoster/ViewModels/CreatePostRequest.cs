using System.ComponentModel.DataAnnotations;

namespace TwitPoster.ViewModels;

public record CreatePostRequest([Required] string Body, string Author);