using Bogus;
using Swashbuckle.AspNetCore.Filters;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.Web.SwaggerExamples.Post;

// ReSharper disable once UnusedType.Global
public class CreatePostExamples : IMultipleExamplesProvider<CreatePostRequest>
{
    public IEnumerable<SwaggerExample<CreatePostRequest>> GetExamples()
    {
        return Enumerable.Range(1, 10).Select(e => SwaggerExample.Create($"Example {e}", GetExampleWithBogus())).ToList();
    }

    private static CreatePostRequest GetExampleWithBogus()
    {
        var faker = new Faker();

        return new CreatePostRequest(faker.Lorem.Paragraph());
    }
}