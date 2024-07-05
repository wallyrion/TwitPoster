using FluentValidation;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.Web.Validators;

public sealed class UpdateUserProfileRequestValidator : AbstractValidator<UpdateUserProfileRequest>
{
    public UpdateUserProfileRequestValidator()
    {
        RuleFor(e => e.BirthDate.Date).NotEmpty().LessThan(DateTime.UtcNow.Date);
        RuleFor(e => e.FirstName).NotEmpty().MaximumLength(300);
        RuleFor(e => e.LastName).NotEmpty().MaximumLength(300);
    }
}