using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TwitPoster.BLL.Common.Options;
using TwitPoster.BLL.Interfaces;
using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly AuthOptions _authOptions;

    public JwtTokenGenerator(IOptions<AuthOptions> authOptions)
    {
        _authOptions = authOptions.Value;
    }

    public string GenerateToken(User user)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_authOptions.Secret)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.UserAccount.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var securityToken = new JwtSecurityToken(
            issuer: _authOptions.Issuer,
            audience: _authOptions.Audience,
            expires: DateTime.UtcNow.Add(_authOptions.Expiration),
            claims: claims,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}