using FluentValidation;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.Web.Validators;

public class CreateUserRequestValidator : AbstractValidator<RegistrationRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(e => e.Email).NotEmpty().EmailAddress().MaximumLength(1000);
        RuleFor(e => e.BirthDate).NotEmpty().LessThan(DateTime.UtcNow.Date);
        RuleFor(e => e.FirstName).NotEmpty().MaximumLength(300);
        RuleFor(e => e.LastName).NotEmpty().MaximumLength(300);

        RuleFor(e => e.Password).NotEmpty()
            .MinimumLength(8).MaximumLength(50)
            .Matches(@"^(?=.*[a-zA-Z])(?=.*\d).+$");
    }
}