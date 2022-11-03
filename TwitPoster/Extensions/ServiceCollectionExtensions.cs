using FluentValidation;
using FluentValidation.AspNetCore;
using TwitPoster.Validators;
using TwitPoster.ViewModels;

namespace TwitPoster.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFluentValidators(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
        return services;
    }
}