using FluentValidation;
using TwitPoster.BLL.Exceptions;
using TwitPoster.Chat;

namespace TwitPoster.BLL.Extensions;

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
