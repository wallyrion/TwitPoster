using AutoFixture;
using Bogus;
using TwitPoster.DAL.Models;

namespace TwitPoster.IntegrationTests.TestData;

public class IntegrationData
{
    public Fixture BaseFixture { get; } = new();

    public void Initialize(int defaultUserId)
    {
        var bogus = new Faker();

        BaseFixture.Customize<DAL.Models.Post>(x => x
            .With(p => p.AuthorId, defaultUserId)
            .With(p => p.LikesCount, 0)
            .With(p => p.Body, bogus.Lorem.Paragraph())
            .Without(p => p.Id)
            .Without(p => p.Author)
            .Without(p => p.Comments)
            .Without(p => p.PostLikes));
    }
    
    public IntegrationData()
    {
        BaseFixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => BaseFixture.Behaviors.Remove(b));
        BaseFixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        BaseFixture.Customize<DAL.Models.User>(x => x.Without(u => u.Id));
        BaseFixture.Customize<UserAccount>(x => x.Without(u => u.Id).Without(u => u.IsBanned));
    }
}