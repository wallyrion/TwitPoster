using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using TwitPoster.DAL.Models;

namespace TwitPoster.DAL.Triggers;

public sealed class PostLikeTrigger(TwitPosterContext dbContext) : IAfterSaveTrigger<PostLike>
{
    public async Task AfterSave(ITriggerContext<PostLike> context, CancellationToken cancellationToken)
    {
        if (context.ChangeType is ChangeType.Modified)
        {
            return;
        }

        await dbContext.Posts.ExecuteUpdateAsync(
            p => p.SetProperty(post => post.LikesCount, 
                post => context.ChangeType == ChangeType.Added ? post.LikesCount + 1 : post.LikesCount - 1),
            cancellationToken: cancellationToken);
    }
}