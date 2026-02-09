using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Domain.Entities
{
    public class PlaybackProgress
    {
        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public Guid EpisodeID { get; set; }
        public TimeSpan Position { get; set;}
        public bool IsFinished { get; set; }
        public DateTime UpdatedAt { get; set; }

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
