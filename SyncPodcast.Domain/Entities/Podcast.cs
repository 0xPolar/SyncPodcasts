using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Domain.Entities
{
    public class Podcast
    {
        private readonly List<Episode> _episodes = new();

        public Guid ID { get; private set; }
        public string Title { get; private set; }
        public string Author { get; private set; }
        public string Description { get; private set; }
        public Uri FeedUrl { get; private set; }
        public Uri? ArtworkUrl { get; private set; }
        public DateTime LastFetchedAt { get; private set; }
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
