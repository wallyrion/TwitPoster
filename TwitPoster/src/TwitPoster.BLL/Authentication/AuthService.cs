﻿using LanguageExt.Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TwitPoster.BLL.Common.Options;
using TwitPoster.BLL.Exceptions;
using TwitPoster.BLL.Interfaces;
using TwitPoster.DAL;
using TwitPoster.DAL.Models;
using TwitPoster.Shared.Contracts;

namespace TwitPoster.BLL.Authentication;

public class AuthService(
    IJwtTokenGenerator tokenGenerator,
    TwitPosterContext context,
    IPublishEndpoint publishEndpoint,
    IOptions<ApplicationOptions> appOptions
    )
    : IAuthService
{
    public async Task<string> Login(string email, string password)
    {
        var user = await context.Users.Include(u => u.UserAccount).FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || user.UserAccount.Password != password)
        {
            throw new TwitPosterValidationException("Your password or email is incorrect");
        }

        if (!user.UserAccount.IsEmailConfirmed)
        {
            throw new TwitPosterValidationException("Your email is not confirmed. Please follow email instructions");
        }

        var accessToken = tokenGenerator.GenerateToken(user);

        return accessToken;
    }

    public async Task<Result<int>> Register(string firstName, string lastName, DateTime birthDate, string email, string password)
    {
        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (existingUser != null)
        {
            return new Result<int>(new TwitPosterValidationException("User with this email already exists"));
        }

        var user = new User
        {
            CreatedAt = DateTime.UtcNow,
            Email = email,
            BirthDate = birthDate,
            FirstName = firstName,
            LastName = lastName,
            UserAccount = new UserAccount
            {
                Password = password,
                Role = UserRole.User,
                EmailConfirmationToken = Guid.NewGuid()
            }
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var confirmationUrl = $"{appOptions.Value.TwitPosterUrl}/Auth/EmailConfirmation?Token={user.UserAccount.EmailConfirmationToken}";
        
        var emailBody = $"""
            <h1> You are on the way! </h1>
            <h2> Please confirm your email by clicking on the link below </h2>
            <a href="{confirmationUrl}">Press to confirm email address</a>
        """;

        var mailCommand = new EmailCommand(email, "Welcome to TwitPoster! Confirm your email", emailBody, TextFormat.Html);

        await publishEndpoint.Publish(mailCommand);

        return user.Id;
    }

    public async Task ConfirmEmail(Guid token)
    {
        var user = await context.Users.Include(u => u.UserAccount).FirstOrDefaultAsync(u => u.UserAccount.EmailConfirmationToken == token);

        if (user == null)
        {
            throw new TwitPosterValidationException("Not valid token");
        }

        user.UserAccount.IsEmailConfirmed = true;
        user.UserAccount.EmailConfirmationToken = default;
        await context.SaveChangesAsync();
    }

    public async Task<string> LoginWithGoogle(string email, string firstName, string lastName, bool isEmailConfirmed, string payloadPicture)
    {
        var user = await context.Users.Include(u => u.UserAccount).FirstOrDefaultAsync(u => u.Email == email);

        if (user != null)
        {
            if (!isEmailConfirmed)
            {
                
                user.UserAccount.IsEmailConfirmed = true;
            }
            
            if (user.PhotoUrl == null)
            {
                user.PhotoUrl = payloadPicture;
                user.ThumbnailPhotoUrl = payloadPicture;
            }
        }
        else
        {
            user = new User
            {
                CreatedAt = DateTime.UtcNow,
                Email = email,
                BirthDate = DateTime.Today,
                FirstName = firstName,
                LastName = lastName,
                PhotoUrl = payloadPicture,
                ThumbnailPhotoUrl = payloadPicture,
                UserAccount = new UserAccount
                {
                    Password = Guid.NewGuid().ToString(),
                    Role = UserRole.User,
                    EmailConfirmationToken = default
                }
            };
            
            context.Users.Add(user);
        }
        
        await context.SaveChangesAsync();
        var accessToken = tokenGenerator.GenerateToken(user);
        return accessToken;
    }
}