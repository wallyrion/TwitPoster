using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using TwitPoster.BLL.Interfaces;

namespace TwitPoster.Web.AI.TagsGeneration;

public class TagsGenerator(ChatHistory history, Kernel kernel) : ITagsGenerator
{
    public async Task<List<string>> GenerateTagsFromContentAsync(string text)
    {
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        
        history.AddUserMessage(text);

        var result = await chatCompletionService.GetChatMessageContentAsync(
            history,
            kernel: kernel);

        history.RemoveAt(1);
        var tags = JsonSerializer.Deserialize<List<string>>(result.Content ?? "[]");
            
        return tags ?? [];
    }
}

public class FakeTagsGenerator : ITagsGenerator
{
    public Task<List<string>> GenerateTagsFromContentAsync(string text)
    {
        return Task.FromResult<List<string>>([]);
    }
}