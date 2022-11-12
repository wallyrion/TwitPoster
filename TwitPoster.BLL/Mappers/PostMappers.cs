using TwitPoster.BLL.DTOs;
using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Mappers;

public static class PostMappers
{
    // public static PostDto ToDto(this Post source, int likesCount, bool isLiked)
    // {
    //     return new PostDto(source.Id, source.Body, source.CreatedAt, source.Author.FirstName, source.Author.LastName,
    //         source.Author.Id, likesCount, isLiked);
    // }
    
    public static PostDto ToDto(this Post source, int currentUserId)
    {
        return new PostDto(
            source.Id,
            source.Body,
            source.CreatedAt,
            source.Author.FirstName,
            source.Author.LastName,
            source.Author.Id,
            source.PostLikes.Count,
            source.PostLikes.Any(l => l.UserId == currentUserId),
            source.Comments.Count
            );
    }

    public static PostCommentDto ToDto(this PostComment source)
    {
        return new PostCommentDto
        {
            Author = source.Author.ToAuthorDto(),
            Id = source.Id,
            Text = source.Text,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }
}