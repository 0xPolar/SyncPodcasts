using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Domain.Entities
{
    public class Subscription
    {
        public Guid ID { get; private set; }
        public Guid UserID { get; private set; }
        public Guid PodcastID { get; private set; }
        public DateTime SubscribedAt { get; private set; }

        public Subscription(Guid userId, Guid podcastId)
        {
            ID = Guid.NewGuid();
            UserID = userId;
            PodcastID = podcastId;
            SubscribedAt = DateTime.UtcNow;
        }

        private Subscription() {}
    }
}
