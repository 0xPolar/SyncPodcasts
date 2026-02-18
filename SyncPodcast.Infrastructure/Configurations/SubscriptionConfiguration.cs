using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyncPodcast.Domain.Entities;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Infrastructure.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(s => s.ID);

        // Composite unique constraint to prevent duplicate subscriptions for the same user and podcast
        builder.HasIndex(s => new { s.UserID, s.PodcastID }).IsUnique();

        // Index on UserID for faster lookups of a user's subscriptions
        builder.HasIndex(s => s.UserID);
    }
}
