using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;

namespace TwitPoster.Web;

public class DetectTagsPlugin
{
    [KernelFunction("detect_tags")]
    [Description("Analyse specified content and create tags that describe it. Can provide up to 5 tags. Tag can be 1-2 words long. Your should provide list of tags in tags parameter.")]
    [return: Description("Tags that describe the content")]
    public List<string> GetTags(string text, List<string> tags)
    {
        return tags;
    }
}
