using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyncPodcast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Infrastructure.Configurations;

public class PlaybackProgressConfiguration : IEntityTypeConfiguration<PlaybackProgress>
{
    public void Configure(EntityTypeBuilder<PlaybackProgress> builder)
    {
        builder.HasKey(p => p.ID);

        // Composite unique constraint to ensure one progress record per user and episode
        builder.HasIndex(p => new { p.UserID, p.EpisodeID }).IsUnique();

        // Index on UserID for faster lookups of a user's playback progress
        builder.HasIndex(p => p.UserID);
    }
}
