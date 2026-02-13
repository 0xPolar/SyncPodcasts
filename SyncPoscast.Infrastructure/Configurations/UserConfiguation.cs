using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyncPodcast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPoscast.Infrastructure.Configurations;

public class  UserConfiguation : IEntityTypeConfiguration<User> 
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.ID);

        builder.Property(u => u.Username).IsRequired().HasMaxLength(26);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.CreatedAt).IsRequired();
    }
    
}
