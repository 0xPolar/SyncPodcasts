using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyncPodcast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPoscast.Infrastructure.Configurations;

public class EpisodeConfiguation : IEntityTypeConfiguration<Episode>
{
    public void Configure(EntityTypeBuilder<Episode> builder)
    {
        builder.HasKey(e => e.ID);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.MediaUrl)
            .HasConversion(
                v => v.ToString(),
                v => new Uri(v))
            .HasMaxLength(2000)
            .IsRequired();

        builder.HasIndex(e => e.PodcastID);
    }
}
