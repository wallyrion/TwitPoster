using FluentValidation;
using TwitPoster.ViewModels;

namespace TwitPoster.Validators;

public class CreateUserRequestValidator : AbstractValidator<RegistrationRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(e => e.Email).NotEmpty().EmailAddress();
        RuleFor(e => e.BirthDate).NotEmpty().LessThan(DateTime.UtcNow.Date);
        RuleFor(e => e.FirstName).NotEmpty().MaximumLength(300);
        RuleFor(e => e.LastName).NotEmpty().MaximumLength(300);
    }
}