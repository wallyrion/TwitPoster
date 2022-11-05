using FluentAssertions;
using FluentValidation.TestHelper;
using TwitPoster.Web.Validators;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.Web.Tests.Validators;

public class CreateUserRequestValidatorTests
{
    private CreateUserRequestValidator validator;

    public CreateUserRequestValidatorTests()
    {
        validator = new CreateUserRequestValidator();
    }
    
    [Fact]
    public void Validate_Should_Not_Have_Error_When_Valid()
    {
        // Arrange
        var request = new RegistrationRequest("test", "test", DateTime.UtcNow.AddYears(-1), "test@email.com", "Very Secured Password 1312");
        
        // Act
        var validationResult = validator.TestValidate(request);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void Validate_Should_Have_Error_When_Invalid_Email()
    {
        // Arrange
        var request = new RegistrationRequest("test", "test", DateTime.UtcNow.AddYears(-1), "111", "test");
        
        // Act
        var validationResult = validator.TestValidate(request);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(e => e.Email);
    }
    
    [Fact]
    public void Validate_Should_Have_Password_Error_When_Min_Length_Not_Enough()
    {
        // Arrange
        var request = new RegistrationRequest("test", "test", DateTime.UtcNow.AddYears(-1), "111", "test");
        
        // Act
        var validationResult = validator.TestValidate(request);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(e => e.Password);
    }
    
    [Fact]
    public void Validate_Should_Have_Password_Error_When_Max_Length_Not_Valid()
    {
        // Arrange
        var request = new RegistrationRequest("test", "test", DateTime.UtcNow.AddYears(-1), "111", "qwertyuiopqwerqwertyuiopqwerqwertyuiopqwerqwertyuiopqwerqwertyuiopqwerqwertyuiopqwerqwertyuiopqwer");
        
        // Act
        var validationResult = validator.TestValidate(request);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(e => e.Password);
    }
    
    [Fact]
    public void Validate_Should_Have_Password_Error_When_No_Digits()
    {
        // Arrange
        var request = new RegistrationRequest("test", "test", DateTime.UtcNow.AddYears(-1), "111", "qwertyuiopqwer");
        
        // Act
        var validationResult = validator.TestValidate(request);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(e => e.Password);
    }
    
    [Theory]
    [InlineData("qwer")]
    [InlineData("12321321312312312312")]
    [InlineData("11")]
    [InlineData("AAAAAAAAAAAAAAAA")]
    [InlineData("Hey password2132131 Hey password2132131Hey password2132131Hey password2132131Hey password2132131Hey password2132131")]
    public void Validate_Should_Have_Password_Error_When_InvalidPassword(string password)
    {
        // Arrange
        var request = new RegistrationRequest("test", "test", DateTime.UtcNow.AddYears(-1), "111", password);
        
        // Act
        var validationResult = validator.TestValidate(request);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(e => e.Password);
    }
    
    [Theory]
    [InlineData("email", "Strong_password123", "2021-01-01", "First", "Last")]
    [InlineData("email@gnmail.com", "not", "2021-01-01", "First", "Last")]
    [InlineData("email@gnmail.com", "Strong_password123", "2023-01-01", "First", "Last")]
    [InlineData("email@gnmail.com", "Strong_password123", "2021-01-01", "", "Last")]
    [InlineData("email@gnmail.com", "Strong_password123", "2021-01-01", "First", "")]
    public void Validate_Should_Have_ValidationError_When_Input_Is_Invalid(string email, string password, DateTime birthDate, string firstName, string lastName)
    {
        // Arrange
        var request = new RegistrationRequest(firstName, lastName, birthDate, email, password);
        
        // Act
        var validationResult = validator.TestValidate(request);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }
}