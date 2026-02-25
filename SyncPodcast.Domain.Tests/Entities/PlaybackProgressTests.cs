using SyncPodcast.Domain.Entities;

namespace SyncPodcast.Domain.Tests.Entities;

public class PlaybackProgressTests
{
    [Fact]
    public void Constructor_WithValidArgs_SetsAllProperties()
    {
        var progress = new PlaybackProgress(
            Guid.NewGuid(), Guid.NewGuid(), TimeSpan.Zero, false);

        Assert.NotEqual(Guid.Empty, progress.ID);
        Assert.NotEqual(Guid.Empty, progress.UserID);
        Assert.Equal(TimeSpan.Zero, progress.Position);
    }

    [Fact]
    public void UpdatePosition_WhenBelow90Percent_DoesNotSetFinished()
    {
        var progress = new PlaybackProgress(
            Guid.NewGuid(), Guid.NewGuid(), TimeSpan.Zero, false);

        var duration = TimeSpan.FromMinutes(60);
        var position = TimeSpan.FromMinutes(50); // 83% — below threshold

        progress.UpdatePosition(position, duration);

        Assert.False(progress.IsFinished);
        Assert.Equal(position, progress.Position);
    }

    [Fact]
    public void UpdatePosition_WhenExactly90Percent_SetsIsFinished()
    {
        var progress = new PlaybackProgress(
            Guid.NewGuid(), Guid.NewGuid(), TimeSpan.Zero, false);

        var duration = TimeSpan.FromMinutes(60);
        var position = TimeSpan.FromMinutes(54); // 90% — exact threshold

        progress.UpdatePosition(position, duration);

        Assert.True(progress.IsFinished);
        Assert.Equal(position, progress.Position);
    }
}
