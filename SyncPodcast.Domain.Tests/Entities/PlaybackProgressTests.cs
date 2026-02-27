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

    [Fact]
    public void UpdatePosition_WhenAbove90Percent_SetsIsFinished()
    {
        var progress = new PlaybackProgress(
            Guid.NewGuid(), Guid.NewGuid(), TimeSpan.Zero, false);

        var duration = TimeSpan.FromMinutes(60);
        var position = TimeSpan.FromMinutes(57); // 95% — above threshold

        progress.UpdatePosition(position, duration);

        Assert.True(progress.IsFinished);
        Assert.Equal(position, progress.Position);
    }

    [Fact]
    public void UpdatePosition_WhenDurationIsZero_DoesNotSetFinished()
    {
        var progress = new PlaybackProgress(
            Guid.NewGuid(), Guid.NewGuid(), TimeSpan.Zero, false);

        progress.UpdatePosition(TimeSpan.FromMinutes(30), TimeSpan.Zero);

        Assert.False(progress.IsFinished);
    }

    [Fact]
    public void UpdatePosition_WhenAlreadyFinished_StaysFinished()
    {
        var progress = new PlaybackProgress(
            Guid.NewGuid(), Guid.NewGuid(), TimeSpan.Zero, true);

        // 1 minute out of 60 = 1.6% — well below the 90% threshold
        progress.UpdatePosition(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(60));

        Assert.True(progress.IsFinished);
    }
}
