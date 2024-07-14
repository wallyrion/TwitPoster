using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Domain.Common.Exceptions;

namespace TwitPoster.Chat.Infrastructure.Common;

public static class OptionsExtensions
{
    public static TOption BindOption<TOption>(this IConfiguration configuration, IServiceCollection serviceCollection, bool shouldBeRegisteredInServices = true)
        where TOption : class, ITwitposterOptions<TOption>
    {
        var sectionName = TOption.SectionName;
        IConfigurationSection config = configuration.GetRequiredSection(sectionName);

        var options = config.Get<TOption>()!;

        var validationResult = options.Validate();
        if (!validationResult.IsValid)
        {
            throw new TwitPosterValidationException($"Error while validating config {TOption.SectionName}", new ValidationException(validationResult.Errors));
        }

        if (shouldBeRegisteredInServices)
        {
            serviceCollection.AddOptions<TOption>()
                .BindConfiguration(sectionName)
                .ValidateOnStart();
        }
        
        return options;
    }
}
