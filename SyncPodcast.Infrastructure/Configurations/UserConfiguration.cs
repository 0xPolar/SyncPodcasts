using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyncPodcast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.ID);

        builder.Property(u => u.Username).IsRequired().HasMaxLength(26);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.CreatedAt).IsRequired();

        builder.Property(u => u.RefreshToken).HasMaxLength(128);
        builder.Property(u => u.RefreshTokenExpiry);

        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
    }
    
}
