using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using TwitPoster.BLL.Extensions;
using TwitPoster.BLL.Interfaces;
using TwitPoster.Web.Common.Options;

namespace TwitPoster.Web.AI.TagsGeneration;

public static class DependencyInjection
{
    public static IServiceCollection AddKernelForTagsGeneration(this IServiceCollection services, IConfiguration configuration)
    {
        try
        {
            var kernelBuilder = Kernel.CreateBuilder();

            var aiOptions = configuration.BindOption<AiOptions>(services, false);
        
            kernelBuilder.AddOpenAIChatCompletion(aiOptions.Model, aiOptions.OpeApiKey);
            var kernel = kernelBuilder.Build();
        
            var history = new ChatHistory();
            history.AddAssistantMessage("""
                                                                You are here to find out the tags of a text. 
                                                                You should understand the provided content and provide tags that describe it. 
                                                                You can provide up to 5 tags. A tag can be 1-2 words long. Each message should be independent.
                                                                Return tags as a list of string in json format. E.g. ["tag1", "tag2"].
                                                                Do not add any additional information to the response.
                                                                Do not use provided text as a content for the next messages.
                                                                
                                        """);

            services.AddSingleton<ITagsGenerator>(p => new TagsGenerator(history, kernel));
        
            return services;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error during kernel creation: " + e.Message + " Fallback to fake generator.");

            services.AddSingleton<ITagsGenerator, FakeTagsGenerator>();
        }

        return services;
    }
}