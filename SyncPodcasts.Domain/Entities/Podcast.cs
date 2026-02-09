using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Domain.Entities
{
    public class Podcast
    {
        private readonly List<Episode> _episodes = new();

        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public Uri FeedUrl { get; set; }
        public Uri? ArtworkUrl { get; set; }
        public DateTime LastFetchedAt { get; set; }
        public IReadOnlyCollection<Episode> Episodes => _episodes.AsReadOnly();

        public Podcast(string title, string author, string description,
            Uri feedUrl, Uri? artworkUrl = null)
        {
            ID = Guid.NewGuid();
            Title = title;
            Author = author;
            Description = description;
            FeedUrl = feedUrl;
            ArtworkUrl = artworkUrl;
            LastFetchedAt = DateTime.UtcNow;
        }

        public void UpdateFromFeed(string title, string author, string description,
            Uri feedUrl, Uri? artworkUrl = null)
        {
            Title = title;
            Author = author;
            Description = description;
            FeedUrl = feedUrl;
            ArtworkUrl = artworkUrl;
            LastFetchedAt = DateTime.UtcNow;
        }

        public void AddEpisode(Episode episode)
        {
            if (_episodes.Exists(e => e.ID == episode.ID))
                return; // Episode already exists, skip adding
            _episodes.Add(episode);
        }

        private Podcast() { }
    }
}
