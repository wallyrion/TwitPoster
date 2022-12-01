using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}