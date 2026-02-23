using SyncPodcast.Domain.Entities;

namespace SyncPodcast.Domain.Tests.Helpers;
public static class EntityFactory
{
  public static User CreateUser(
      Guid? id = null,
      string username = "testuser",
      string email = "test@example.com",
      string passwordHash = "hashed-password",
      DateTime? createdAt = null)
  {
      return new User(
          id ?? Guid.NewGuid(),
          username,
          email,
          passwordHash,
          createdAt ?? DateTime.UtcNow);
  }

  public static Podcast CreatePodcast(
      string title = "Test Podcast",
      string author = "Test Author",
      string description = "A test podcast",
      string feedUrl = "https://example.com/feed.xml",
      string? artworkUrl = null)
  {
      return new Podcast(
          title, author, description,
          new Uri(feedUrl),
          artworkUrl != null ? new Uri(artworkUrl) : null);
  }

  public static Episode CreateEpisode(
      Guid? podcastId = null,
      string title = "Test Episode",
      string description = "A test episode",
      string mediaUrl = "https://example.com/ep1.mp3",
      TimeSpan? duration = null,
      DateTime? publishedAt = null)
  {
      return new Episode(
          podcastId ?? Guid.NewGuid(),
          title, description,
          new Uri(mediaUrl),
          duration ?? TimeSpan.FromMinutes(30),
          publishedAt ?? DateTime.UtcNow);
  }

  public static PlaybackProgress CreatePlaybackProgress(
      Guid? userId = null,
      Guid? episodeId = null,
      TimeSpan? position = null,
      bool isFinished = false)
  {
      return new PlaybackProgress(
          userId ?? Guid.NewGuid(),
          episodeId ?? Guid.NewGuid(),
          position ?? TimeSpan.Zero,
          isFinished);
  }

  public static Subscription CreateSubscription(
      Guid? userId = null,
      Guid? podcastId = null)
  {
      return new Subscription(
          userId ?? Guid.NewGuid(),
          podcastId ?? Guid.NewGuid());
  }
}
