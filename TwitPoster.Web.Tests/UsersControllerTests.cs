using FluentAssertions;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TwitPoster.BLL.Exceptions;
using TwitPoster.BLL.Interfaces;
using TwitPoster.Web.Controllers;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.Web.Tests;

public class AuthControllerTests
{
    private readonly Mock<IUsersService> _userServiceMock = new();
    private readonly AuthController _sut;
    public AuthControllerTests()
    {
        _sut = new AuthController(_userServiceMock.Object);
    }
    
    [Fact]
    public async Task Register_Should_Save_Correctly()
    {
        // Arrange
        const int expectedUserId = 100;
        
        var registrationRequest = new RegistrationRequest("First", "Last", DateTime.UtcNow.AddYears(-1), "kornienko1296@gmail.com", "password");
        
        _userServiceMock.Setup(e => e.Register(registrationRequest.FirstName, registrationRequest.LastName, registrationRequest.BirthDate, registrationRequest.Email, registrationRequest.Password))
            .ReturnsAsync(expectedUserId);
        
        // Act
        var result = await _sut.Register(registrationRequest);

        // Assert
        result.Should().BeOfType<ContentResult>().Which.Content.Should().MatchRegex("^Registration successful.*");
    }
    
    [Fact]
    public async Task Register_Should_Return_BadRequest_When_Service_Returns_Error()
    {
        // Arrange
        var registrationRequest = new RegistrationRequest("First", "Last", DateTime.UtcNow.AddYears(-1), "kornienko1296@gmail.com", "password");

        _userServiceMock.Setup(e => e.Register(registrationRequest.FirstName, registrationRequest.LastName, registrationRequest.BirthDate, registrationRequest.Email, registrationRequest.Password))
            .ReturnsAsync( new Result<int>(new TwitPosterValidationException("Invalid email")));
        
        // Act
        var result = await _sut.Register(registrationRequest);

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        var problemDetails = objectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails!.Title.Should().Be("Invalid email");
    }
    
    [Fact]
    public async Task Login_Should_Return_Token_Correctly()
    {
        // Arrange
        const int expectedUserId = 100;
        const string expectedToken = "SecretToken";
        var usersServiceMock = new Mock<IUsersService>();
        var usersService = usersServiceMock.Object;
        
        var sut = new AuthController(usersService);

        var loginRequest = new LoginRequest("kornienko1296@gmail.com", "password");
        
        usersServiceMock.Setup(e => e.Login(loginRequest.Email, loginRequest.Password))
            .ReturnsAsync(expectedToken);
        
        // Act
        var result = await sut.Login(loginRequest);

        var okObjectResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var resultValue = okObjectResult!.Value.Should().BeOfType<string>().Subject;

        // Assert
        resultValue.Should().Be(expectedToken);
    }
}