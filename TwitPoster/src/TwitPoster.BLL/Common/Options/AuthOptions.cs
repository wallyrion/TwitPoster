namespace TwitPoster.BLL.Common.Options;

public sealed class AuthOptions
{
    public required string Issuer { get; set; }
    
    public required string Audience { get; set; }
    
    public required string Secret { get; set; }
    
    public required TimeSpan Expiration { get; set; }
}