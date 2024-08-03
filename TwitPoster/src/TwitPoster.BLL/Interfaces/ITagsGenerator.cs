namespace TwitPoster.BLL.Interfaces;

public interface ITagsGenerator
{
    Task<List<string>> GenerateTagsFromContentAsync(string text);
}