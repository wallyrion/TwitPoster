using AutoFixture;
using Bogus;
using TwitPoster.DAL;
using TwitPoster.DAL.Models;

namespace TwitPoster.IntegrationTests.TestData;

public class IntegrationData
{
    public Fixture BaseFixture { get; } = new();

    private readonly TwitPosterContext _dbContext;
    
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
    
    public IntegrationData(TwitPosterContext dbContext)
    {
        _dbContext = dbContext;
        BaseFixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => BaseFixture.Behaviors.Remove(b));
        BaseFixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        BaseFixture.Customize<User>(x => x.Without(u => u.Id));
        BaseFixture.Customize<UserAccount>(x => x.Without(u => u.Id).Without(u => u.IsBanned));
    }
    
    public async Task<IReadOnlyList<T>> AddMany<T>(int count = 3) where T : class
    {
        var entities = BaseFixture.CreateMany<T>(count).ToList();
        _dbContext.Set<T>().AddRange(entities);
        await _dbContext.SaveChangesAsync();

        return entities;
    }
    
    public async Task<IReadOnlyList<T>> AddMany<T>(IReadOnlyList<T> entitiesToAdd) where T : class
    {
        _dbContext.Set<T>().AddRange(entitiesToAdd);
        await _dbContext.SaveChangesAsync();
        return entitiesToAdd;
    }
}