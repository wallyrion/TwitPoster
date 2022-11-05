using TwitPoster.BLL.DTOs;
using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Mappers;

public static class PostMappers
{
    public static PostDto ToDto(this Post source) => 
        new PostDto(source.Id, source.Body, source.CreatedAt, source.Author.FirstName, source.Author.LastName, source.Author.Id);
}