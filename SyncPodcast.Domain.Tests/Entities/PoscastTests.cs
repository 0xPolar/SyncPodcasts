using SyncPodcast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Domain.Tests.Entities;

public class PodcastTests
{
    [Fact]
    public void Constructor_WithValidArgs_SetsPropertiesAndGeneratesId()
    {
        Podcast podcast = new Podcast(
            "My Pod", "Author", "Description",
            new Uri("https://example.com/feed.xml"),
            new Uri("https://example.com/art.jpg"));

        Assert.NotEqual(Guid.Empty, podcast.ID);
        Assert.Equal("My Pod", podcast.Title);
        Assert.Equal("Author", podcast.Author);
        Assert.Empty(podcast.Episodes); // starts with no episodes
    }

    [Fact]
    public void AddEpisode_WithNewEpisode_AddsToCollection()
    {
        Podcast podcast = new Podcast(
            "My Pod", "Author", "Description",
            new Uri("https://example.com/feed.xml"),
            new Uri("https://example.com/art.jpg"));

        Episode episode = new Episode(podcast.ID, "Episode 1", "Desc", new Uri("https://example.com/ep1.mp3"), TimeSpan.FromMinutes(30), DateTime.UtcNow);

        podcast.AddEpisode(episode);

        Assert.Single(podcast.Episodes);

    }

    [Fact]
    public void AddEpisode_WithDuplicateId_DoesNotAddSecondTime()
    {
        Podcast podcast = new Podcast(
            "My Pod", "Author", "Description",
            new Uri("https://example.com/feed.xml"),
            new Uri("https://example.com/art.jpg"));

        Episode episode = new Episode(podcast.ID, "Episode 1", "Desc", new Uri("https://example.com/ep1.mp3"), TimeSpan.FromMinutes(30), DateTime.UtcNow);

        podcast.AddEpisode(episode);
        podcast.AddEpisode(episode);

        Assert.Single(podcast.Episodes);
    }

}
