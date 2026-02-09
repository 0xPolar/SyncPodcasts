using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Domain.Entities
{
    public class Episode
    {
        public Guid ID { get; set; }
        public Guid PodcastID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Uri AudioUrl { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime PublishedAt { get; set; }

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
