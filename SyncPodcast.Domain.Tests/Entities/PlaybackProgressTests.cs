using SyncPodcast.Domain.Entities;

namespace SyncPodcast.Domain.Tests.Entities;

public class PlaybackProgressTests
{
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
}
