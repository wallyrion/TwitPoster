﻿using Mapster;
using TwitPoster.BLL.DTOs;
using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Mappers;

public class PostMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Post, PostDto>()
            .Map(dest => dest.AuthorFirstName, src => src.Author.FirstName)
            .Map(dest => dest.AuthorLastName, src => src.Author.LastName)
            .Map(dest => dest.AuthorPhotoUrl, src => src.Author.PhotoUrl)
            .Map(dest => dest.AuthorId, src => src.Author.Id)
            .Map(dest => dest.LikesCount, src => src.LikesCount)
            .Map(dest => dest.CommentsCount, src => src.Comments.Count)
            ;
    }
}