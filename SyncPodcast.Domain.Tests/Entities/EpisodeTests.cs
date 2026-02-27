using SyncPodcast.Domain.Entities;

namespace SyncPodcast.Domain.Tests.Entities;

public class EpisodeTests
{
    [Fact]
    public void Constructor_WithValidArgs_SetsPropertiesAndGeneratesId()
    {
        var podcastId = Guid.NewGuid();
        var title = "Episode 1";
        var description = "A great episode";
        var mediaUrl = new Uri("https://example.com/ep1.mp3");
        var duration = TimeSpan.FromMinutes(30);
        var publishedAt = DateTime.UtcNow;

        var episode = new Episode(podcastId, title, description, mediaUrl, duration, publishedAt);

        Assert.NotEqual(Guid.Empty, episode.ID);
        Assert.Equal(podcastId, episode.PodcastID);
        Assert.Equal(title, episode.Title);
        Assert.Equal(description, episode.Description);
        Assert.Equal(mediaUrl, episode.MediaUrl);
        Assert.Equal(duration, episode.Duration);
        Assert.Equal(publishedAt, episode.PublishedAt);
    }
}
