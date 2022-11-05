using Swashbuckle.AspNetCore.Filters;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.Web.SwaggerExamples.User;

public class LoginRequestExamples : IMultipleExamplesProvider<LoginRequest>
{
    public IEnumerable<SwaggerExample<LoginRequest>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Example 1",
            new LoginRequest( "john.doe@google.com", "Qwerty123"));
    }
}