using FluentAssertions;
using MassTransit.SqlTransport;
using Microsoft.EntityFrameworkCore;
using TwitPoster.IntegrationTests.Extensions;
using TwitPoster.IntegrationTests.Fixtures;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.IntegrationTests.User;

public class UpdateUserProfileTests(IntegrationTestWebFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Should_UpdateUserProfile()
    {
        await AddAuthorization();
        
        var request = new UpdateUserProfileRequest("New Name", "New Bio", DateTime.UtcNow.AddYears(-20));
        
        var response = await ApiClient.PutAsJsonAsync("users/profile", request);
        response.Should().Be204NoContent();

        var user = await DbContext.Users.FirstAsync(u => u.Id == DefaultUserId);
        user.FirstName.Should().Be(request.FirstName);
        user.LastName.Should().Be(request.LastName);
        user.BirthDate.Should().Be(DateTime.UtcNow.Date.AddYears(-20));
    }
}