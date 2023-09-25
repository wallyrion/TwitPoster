using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwitPoster.BLL.Common.Options;

namespace TwitPoster.BLL.Extensions;

public static class OptionsExtensions
{
    public static TOption BindOption<TOption>(this IConfiguration configuration, IServiceCollection serviceCollection, bool configure = true)
        where TOption : class, ITwitposterOptions<TOption>
    {
        var sectionName = TOption.SectionName;
        IConfigurationSection config = configuration.GetRequiredSection(sectionName);

        var options = config.Get<TOption>()!;

        var validationResult = options.Validate();
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        if (configure)
        {
            serviceCollection.AddOptions<TOption>()
                .BindConfiguration(sectionName)
                .ValidateOnStart();
        }
        
        return options;
    }
}
