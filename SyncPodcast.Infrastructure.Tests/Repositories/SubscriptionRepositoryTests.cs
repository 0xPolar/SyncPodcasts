using FluentAssertions;
using SyncPodcast.Domain.Tests.Helpers;
using SyncPodcast.Infrastructure.Repositories;
using SyncPodcast.Infrastructure.Tests.Fixtures;

namespace SyncPodcast.Infrastructure.Tests.Repositories;

public class SubscriptionRepositoryTests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture _fixture;

    public SubscriptionRepositoryTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task AddAsync_ThenExistsAsync_ReturnsTrue()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new SubscriptionRepository(context);
        var subscription = EntityFactory.CreateSubscription();

        await repo.AddAsync(subscription, CancellationToken.None);

        var exists = await repo.ExistsAsync(subscription.UserID, subscription.PodcastID, CancellationToken.None);
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenNotSubscribed_ReturnsFalse()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new SubscriptionRepository(context);

        var exists = await repo.ExistsAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        exists.Should().BeFalse();
    }

    [Fact]
    public async Task GetUserPodcastsAsync_ReturnsSubscribedPodcasts()
    {
        await using var context = _fixture.CreateDbContext();
        var subRepo = new SubscriptionRepository(context);
        var podcastRepo = new PodcastRepository(context);

        var userId = Guid.NewGuid();
        var podcast = EntityFactory.CreatePodcast(feedUrl: $"https://example.com/{Guid.NewGuid():N}.xml");
        await podcastRepo.AddAsync(podcast, CancellationToken.None);
        await subRepo.AddAsync(new Domain.Entities.Subscription(userId, podcast.ID), CancellationToken.None);

        var podcasts = await subRepo.GetUserPodcastsAsync(userId, CancellationToken.None);

        podcasts.Should().HaveCount(1);
        podcasts[0].ID.Should().Be(podcast.ID);
    }

    [Fact]
    public async Task GetUserPodcastsAsync_WhenNoSubscriptions_ReturnsEmptyList()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new SubscriptionRepository(context);

        var podcasts = await repo.GetUserPodcastsAsync(Guid.NewGuid(), CancellationToken.None);

        podcasts.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_RemovesSubscription()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new SubscriptionRepository(context);
        var subscription = EntityFactory.CreateSubscription();
        await repo.AddAsync(subscription, CancellationToken.None);

        await repo.DeleteAsync(subscription.UserID, subscription.PodcastID, CancellationToken.None);

        var exists = await repo.ExistsAsync(subscription.UserID, subscription.PodcastID, CancellationToken.None);
        exists.Should().BeFalse();
    }
}
