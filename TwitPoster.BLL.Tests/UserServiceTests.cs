using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TwitPoster.BLL.Exceptions;
using TwitPoster.BLL.Interfaces;
using TwitPoster.BLL.Services;
using TwitPoster.DAL;
using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Tests;

public class UserServiceTests
{
    private readonly TwitPosterContext _context;
    private readonly UserService _sut;
    private readonly Fixture _fixture = new();
    
    public UserServiceTests()
    {
        var options = new DbContextOptionsBuilder<TwitPosterContext>()
            .UseInMemoryDatabase($"DB{Guid.NewGuid()}")
            .Options;
        _context = new TwitPosterContext(options);
        _sut = new UserService(_context, new Mock<ICurrentUser>().Object, new Mock<IEmailSender>().Object);
    }
    
    [Fact]
    public async Task Login_Should_Return_AccessToken_WhenEmailIsConfirmed()
    {
        // Arrange
        _fixture.Customize<UserAccount>(composer => composer.With(account => account.IsEmailConfirmed, true));
        var expectedUser = _fixture.Create<User>();
        _context.Users.Add(expectedUser);
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _sut.Login(expectedUser.Email, expectedUser.UserAccount.Password);

        // Assert
        result.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Login_Should_Throw_Error_WhenEmailIsNotConfirmed()
    {
        // Arrange
        _fixture.Customize<UserAccount>(composer => composer.With(account => account.IsEmailConfirmed, false));
        var expectedUser = _fixture.Create<User>();
        _context.Users.Add(expectedUser);
        await _context.SaveChangesAsync();
        
        // Act
        var action = async() => await _sut.Login(expectedUser.Email, expectedUser.UserAccount.Password);

        // Assert
        await action.Should().ThrowAsync<TwitPosterValidationException>().WithMessage("Your email is not confirmed. Please follow email instructions");
    }
    
    [Fact]
    public async Task Login_Should_Throw_Exception_If_User_Exists_But_Wrong_Password()
    {
        // Arrange
        var expectedUser = _fixture.Create<User>();
        _context.Users.Add(expectedUser);
        await _context.SaveChangesAsync();
        
        // Act
        var loginFunc = async () => await _sut.Login(expectedUser.Email, "wrong password");
        
        // Assert
        await loginFunc.Should().ThrowAsync<TwitPosterValidationException>()
            .WithMessage("Your password or email is incorrect");
    }
    
    [Fact]
    public async Task Register_Should_Save_Correct()
    {
        // Act
        var result = await _sut.Register("first", "test test1", DateTime.UtcNow.AddYears(-1), "test@test.com", "143124312");
        
        // Assert
        result.IsSuccess.Should().BeTrue();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@test.com");
        user.Should().NotBeNull();
    }
}