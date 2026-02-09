using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Domain.Entities
{
    public class Episode
    {
        public Guid ID { get; private set; }
        public Guid PodcastID { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public Uri AudioUrl { get; private set; }
        public TimeSpan Duration { get; private set; }
        public DateTime PublishedAt { get; private set; }

        public Episode(Guid podcastId, string title, string description,
            Uri audioUrl, TimeSpan duration, DateTime publishedAt)
        {
            ID = Guid.NewGuid();
            PodcastID = podcastId;
            Title = title;
            AudioUrl = audioUrl;
            Description = description;
            Duration = duration;
            PublishedAt = publishedAt;

        }
        private Episode() { }
    }
}
