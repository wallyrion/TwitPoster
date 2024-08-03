using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace TwitPoster.Web;

public static class ExperimentWithTags
{
    public static async Task Start()
    {
        // Create a kernel with Azure OpenAI chat completion
        var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion("gpt-4o-mini", "");

// Add enterprise components
//builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

// Build the kernel
        Kernel kernel = builder.Build();
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Add a plugin (the LightsPlugin class is defined below)
//kernel.Plugins.AddFromType<LightsPlugin>("Lights");

        var lights = new DetectTagsPlugin();
        kernel.Plugins.AddFromObject(lights,"Tags");

// Enable planning
        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

// Create a history store the conversation
        var history = new ChatHistory();
        history.AddAssistantMessage("You are here to find out the tags of a text. Yoy should understand the provided content and provide tags that describe it. You can provide up to 5 tags. A tag can be 1-2 words long. Each message should be independent.");

// Initiate a back-and-forth chat
        string? userInput;
        do {
            // Collect user input
            Console.Write("User > ");
            userInput = Console.ReadLine();

            if (userInput == "status")
            {
                // Get the status of the lights
             
            }
    
            // Add user input
            history.AddUserMessage(userInput);

            // Get the response from the AI
            var result = await chatCompletionService.GetChatMessageContentAsync(
                history,
                executionSettings: openAIPromptExecutionSettings,
                kernel: kernel);

            // Print the results
            Console.WriteLine("Assistant > " + result);

            // Add the message from the agent to the chat history
            history.AddMessage(result.Role, result.Content ?? string.Empty);
        } while (userInput is not null);
    }
}