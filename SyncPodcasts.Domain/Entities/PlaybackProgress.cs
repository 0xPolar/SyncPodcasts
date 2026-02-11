using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Domain.Entities
{
    public class PlaybackProgress
    {
        public Guid ID { get; private set; }
        public Guid UserID { get; private set; }
        public Guid EpisodeID { get; private set; }
        public TimeSpan Position { get; private set;}
        public TimeSpan EpisodeDuration { get; private set; }
        public bool IsFinished { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public PlaybackProgress(Guid userId, Guid episodeId, TimeSpan position, bool isFinished)
        {
            ID = Guid.NewGuid();
            UserID = userId;
            EpisodeID = episodeId;
            Position = position;
            IsFinished = isFinished;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePosition(TimeSpan position, TimeSpan episodeDuration)
        {
            Position = position;
            UpdatedAt = DateTime.UtcNow;

            if (episodeDuration > TimeSpan.Zero && Position >= episodeDuration * 0.90)
            {
                IsFinished = true;
            }
        }

        private PlaybackProgress() { }
    }
}
