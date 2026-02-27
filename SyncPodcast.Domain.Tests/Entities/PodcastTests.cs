using SyncPodcast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        int length = podcast.Episodes.Count;
        Assert.Equal(1, length);

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

        int length = podcast.Episodes.Count;
        Assert.Equal(1, length);
    }

    [Fact]
    public void AddEpisode_WithMultipleUniqueEpisodes_AddsAll()
    {
        Podcast podcast = new Podcast(
            "My Pod", "Author", "Description",
            new Uri("https://example.com/feed.xml"),
            new Uri("https://example.com/art.jpg"));

        Episode episode1 = new Episode(podcast.ID, "Episode 1", "Desc", new Uri("https://example.com/ep1.mp3"), TimeSpan.FromMinutes(30), DateTime.UtcNow);
        Episode episode2 = new Episode(podcast.ID, "Episode 2", "Desc", new Uri("https://example.com/ep2.mp3"), TimeSpan.FromMinutes(45), DateTime.UtcNow);
        Episode episode3 = new Episode(podcast.ID, "Episode 3", "Desc", new Uri("https://example.com/ep3.mp3"), TimeSpan.FromMinutes(60), DateTime.UtcNow);


        podcast.AddEpisode(episode1);
        podcast.AddEpisode(episode2);
        podcast.AddEpisode(episode3);

        int length = podcast.Episodes.Count;
        Assert.Equal(3, length);
    }

    [Fact]
    public void UpdateFromFeed_WithNewData_UpdatesAllProperties()
    {
        Podcast podcast = new Podcast(
            "My Pod", "Author", "Description",
            new Uri("https://example.com/feed.xml"),
            new Uri("https://example.com/art.jpg"));

        podcast.UpdateFromFeed("New Title", "New Author", "New Description", new Uri("https://example.com/newfeed.xml"), new Uri("https://example.com/newart.jpg"));

        Assert.Equal("New Title", podcast.Title);
        Assert.Equal("New Author", podcast.Author);
        Assert.Equal("New Description", podcast.Description);

    }

}
