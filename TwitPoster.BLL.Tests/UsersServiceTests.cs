using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TwitPoster.BLL.Exceptions;
using TwitPoster.BLL.Services;
using TwitPoster.DAL;
using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Tests;

public class UsersServiceTests
{
    private readonly TwitPosterContext _context;
    private readonly UsersService _sut;
    private readonly Fixture _fixture = new();
    
    public UsersServiceTests()
    {
        var options = new DbContextOptionsBuilder<TwitPosterContext>()
            .UseInMemoryDatabase($"DB{Guid.NewGuid()}")
            .Options;
        _context = new TwitPosterContext(options);
        _sut = new UsersService(_context);
    }
    
    [Fact]
    public async Task Login_Should_Return_Correct_Result()
    {
        // Arrange
        var expectedUser = _fixture.Create<User>();
        _context.Users.Add(expectedUser);
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _sut.Login(expectedUser.Email, expectedUser.Password);

        // Assert
        result.AccessToken.Should().NotBeEmpty();
        result.UserId.Should().Be(expectedUser.Id);
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