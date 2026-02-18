using Microsoft.EntityFrameworkCore;
using SyncPodcast.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Infrastructure.Configurations;
public class PodcastConfiguration : IEntityTypeConfiguration<Podcast>
{
    public void Configure(EntityTypeBuilder<Podcast> builder)
    {
        builder.HasKey(p => p.ID);

        builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Author).IsRequired().HasMaxLength(200);

        builder.Property(p => p.FeedUrl)
            .HasConversion(
                v => v.ToString(),       // C# Uri → DB string
                v => new Uri(v))         // DB string → C# Uri
            .HasMaxLength(2000)
            .IsRequired();

        // Same conversion for the nullable ArtworkUrl
        builder.Property(p => p.ArtworkUrl)
            .HasConversion(
                v => v == null ? null : v.ToString(),
                v => v == null ? null : new Uri(v))
            .HasMaxLength(2000);

        // "No two podcasts can have the same feed URL"
        builder.HasIndex(p => p.FeedUrl).IsUnique();

        // "One Podcast has many Episodes, linked by PodcastID"
        // When you delete a Podcast, all its Episodes get deleted too (Cascade)
        builder.HasMany(p => p.Episodes)
            .WithOne()
            .HasForeignKey(e => e.PodcastID)
            .OnDelete(DeleteBehavior.Cascade);

        // This tells EF Core: "The Episodes property uses a private backing field
        // called _episodes. Populate THAT field when loading from the database."
        // Without this, EF Core can't populate the read-only Episodes collection.
        builder.Navigation(p => p.Episodes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

    }
}
