using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.DTOs;

public class AccountDetailDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
}