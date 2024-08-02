using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace TwitPoster.Web;

public static class ExperimentWithLights
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

        var lights = new LightsPlugin();
        kernel.Plugins.AddFromObject(lights,"Lights");

// Enable planning
        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

// Create a history store the conversation
        var history = new ChatHistory();

// Initiate a back-and-forth chat
        string? userInput;
        do {
            // Collect user input
            Console.Write("User > ");
            userInput = Console.ReadLine();

            if (userInput == "status")
            {
                // Get the status of the lights
                var r = await lights.GetLightsAsync();
                continue;
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