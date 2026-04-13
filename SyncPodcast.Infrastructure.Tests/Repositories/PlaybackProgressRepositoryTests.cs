using FluentAssertions;
using SyncPodcast.Domain.Tests.Helpers;
using SyncPodcast.Infrastructure.Repositories;
using SyncPodcast.Infrastructure.Tests.Fixtures;

namespace SyncPodcast.Infrastructure.Tests.Repositories;

public class PlaybackProgressRepositoryTests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture _fixture;

    public PlaybackProgressRepositoryTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task SaveAsync_NewProgress_CreatesRecord()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new PlaybackProgressRepository(context);
        var progress = EntityFactory.CreatePlaybackProgress(position: TimeSpan.FromMinutes(5));

        await repo.SaveAsync(progress, CancellationToken.None);

        var fetched = await repo.GetAsync(progress.UserID, progress.EpisodeID, CancellationToken.None);
        fetched.Should().NotBeNull();
        fetched!.Position.Should().Be(TimeSpan.FromMinutes(5));
    }

    [Fact]
    public async Task SaveAsync_ExistingProgress_UpdatesRecord()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new PlaybackProgressRepository(context);
        var progress = EntityFactory.CreatePlaybackProgress();
        await repo.SaveAsync(progress, CancellationToken.None);

        progress.UpdatePosition(TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(60));
        await repo.SaveAsync(progress, CancellationToken.None);

        await using var verifyContext = _fixture.CreateDbContext();
        var verifyRepo = new PlaybackProgressRepository(verifyContext);
        var fetched = await verifyRepo.GetAsync(progress.UserID, progress.EpisodeID, CancellationToken.None);
        fetched!.Position.Should().Be(TimeSpan.FromMinutes(15));
    }

    [Fact]
    public async Task GetAsync_WhenNotExists_ReturnsNull()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new PlaybackProgressRepository(context);

        var fetched = await repo.GetAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        fetched.Should().BeNull();
    }
}
