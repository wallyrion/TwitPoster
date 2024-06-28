using Google.Apis.Auth;

namespace TwitPoster.Web.Common.Auth;

public static class GoogleSignInHelper
{
    public static async Task<GoogleJsonWebSignature.Payload?> ValidateGoogleToken(string token, string clientId)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = [clientId],
        };

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
            return payload;
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }
}