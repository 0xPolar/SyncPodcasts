using SyncPodcast.Domain.Entities;

namespace SyncPodcast.Domain.Tests.Entities;

public class SubscriptionTests
{
    [Fact]
    public void Constructor_WithValidArgs_SetsPropertiesAndGeneratesId()
    {
        var userId = Guid.NewGuid();
        var podcastId = Guid.NewGuid();
        var before = DateTime.UtcNow;

        var subscription = new Subscription(userId, podcastId);

        Assert.NotEqual(Guid.Empty, subscription.ID);
        Assert.Equal(userId, subscription.UserID);
        Assert.Equal(podcastId, subscription.PodcastID);
        Assert.True(subscription.SubscribedAt >= before);
        Assert.True(subscription.SubscribedAt <= DateTime.UtcNow);
    }
}
