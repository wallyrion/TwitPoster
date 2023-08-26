using Swashbuckle.AspNetCore.Filters;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.Web.SwaggerExamples.User;

public class LoginRequestExamples : IMultipleExamplesProvider<LoginRequest>
{
    public IEnumerable<SwaggerExample<LoginRequest>> GetExamples()
    {
        var ojb = new
        {
            name = "alex",
            surname = "korniienko"
        };
        
        yield return SwaggerExample.Create(
            "Me",
            new LoginRequest( "oleksii.korniienko@twitposter.com", "Qwerty123"));
        yield return SwaggerExample.Create(
            "Admin",
            new LoginRequest( "admin@twitposter.com", "Qwerty123"));
        yield return SwaggerExample.Create(
            "Moderator",
            new LoginRequest( "moderator@twitposter.com", "Qwerty123"));
    }
}