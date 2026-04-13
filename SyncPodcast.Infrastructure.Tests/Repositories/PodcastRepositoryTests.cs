using FluentAssertions;
using SyncPodcast.Domain.Tests.Helpers;
using SyncPodcast.Infrastructure.Repositories;
using SyncPodcast.Infrastructure.Tests.Fixtures;

namespace SyncPodcast.Infrastructure.Tests.Repositories;

public class PodcastRepositoryTests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture _fixture;

    public PodcastRepositoryTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task AddAsync_ThenGetByIdAsync_ReturnsPodcastWithEpisodes()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new PodcastRepository(context);
        var podcast = EntityFactory.CreatePodcast(
            title: "Add Test",
            feedUrl: $"https://example.com/{Guid.NewGuid():N}.xml");
        podcast.AddEpisode(EntityFactory.CreateEpisode(podcastId: podcast.ID, title: "Ep 1"));
        podcast.AddEpisode(EntityFactory.CreateEpisode(podcastId: podcast.ID, title: "Ep 2"));

        await repo.AddAsync(podcast, CancellationToken.None);

        await using var verifyContext = _fixture.CreateDbContext();
        var verifyRepo = new PodcastRepository(verifyContext);
        var fetched = await verifyRepo.GetPodcastByIdAsync(podcast.ID, CancellationToken.None);

        fetched.Should().NotBeNull();
        fetched!.Title.Should().Be("Add Test");
        fetched.Episodes.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByFeedUrlAsync_WhenExists_ReturnsPodcast()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new PodcastRepository(context);
        var feedUrl = new Uri($"https://example.com/{Guid.NewGuid():N}.xml");
        var podcast = EntityFactory.CreatePodcast(feedUrl: feedUrl.ToString());
        await repo.AddAsync(podcast, CancellationToken.None);

        var fetched = await repo.GetByFeedUrlAsync(feedUrl, CancellationToken.None);

        fetched.Should().NotBeNull();
        fetched!.ID.Should().Be(podcast.ID);
    }

    [Fact]
    public async Task GetByFeedUrlAsync_WhenNotExists_ReturnsNull()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new PodcastRepository(context);

        var fetched = await repo.GetByFeedUrlAsync(
            new Uri($"https://ghost.example.com/{Guid.NewGuid():N}.xml"),
            CancellationToken.None);

        fetched.Should().BeNull();
    }

    [Fact]
    public async Task GetEpisodeByIdAsync_ReturnsEpisode()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new PodcastRepository(context);
        var podcast = EntityFactory.CreatePodcast(feedUrl: $"https://example.com/{Guid.NewGuid():N}.xml");
        var episode = EntityFactory.CreateEpisode(podcastId: podcast.ID, title: "Target Ep");
        podcast.AddEpisode(episode);
        await repo.AddAsync(podcast, CancellationToken.None);

        var fetched = await repo.GetEpisodeByIdAsync(episode.ID, CancellationToken.None);

        fetched.Should().NotBeNull();
        fetched!.Title.Should().Be("Target Ep");
        fetched.PodcastID.Should().Be(podcast.ID);
    }

    [Fact]
    public async Task UpdateAsync_ModifiesPodcast()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new PodcastRepository(context);
        var podcast = EntityFactory.CreatePodcast(
            title: "Original",
            feedUrl: $"https://example.com/{Guid.NewGuid():N}.xml");
        await repo.AddAsync(podcast, CancellationToken.None);

        podcast.UpdateFromFeed("Updated Title", podcast.Author, podcast.Description, podcast.FeedUrl);
        await repo.UpdateAsync(podcast, CancellationToken.None);

        await using var verifyContext = _fixture.CreateDbContext();
        var verifyRepo = new PodcastRepository(verifyContext);
        var fetched = await verifyRepo.GetPodcastByIdAsync(podcast.ID, CancellationToken.None);
        fetched!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task DeleteAsync_CascadesDeleteToEpisodes()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new PodcastRepository(context);
        var podcast = EntityFactory.CreatePodcast(feedUrl: $"https://example.com/{Guid.NewGuid():N}.xml");
        var episode = EntityFactory.CreateEpisode(podcastId: podcast.ID, title: "To Cascade");
        podcast.AddEpisode(episode);
        await repo.AddAsync(podcast, CancellationToken.None);

        await repo.DeleteAsync(podcast.ID, CancellationToken.None);

        await using var verifyContext = _fixture.CreateDbContext();
        var verifyRepo = new PodcastRepository(verifyContext);
        var fetchedPodcast = await verifyRepo.GetPodcastByIdAsync(podcast.ID, CancellationToken.None);
        var fetchedEpisode = await verifyRepo.GetEpisodeByIdAsync(episode.ID, CancellationToken.None);
        fetchedPodcast.Should().BeNull();
        fetchedEpisode.Should().BeNull();
    }
}
