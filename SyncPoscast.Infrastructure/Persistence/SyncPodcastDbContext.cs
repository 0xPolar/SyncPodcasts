using Microsoft.EntityFrameworkCore;
using SyncPodcast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPoscast.Infrastructure.Persistence
{
    public class SyncPodcastDbContext : DbContext 
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Podcast> Podcasts => Set<Podcast>();
        public DbSet<Episode> Episodes => Set<Episode>();
        public DbSet<Subscription> Subscriptions => Set<Subscription>();
        public DbSet<PlaybackProgress> playbackProgresses => Set<PlaybackProgress>();

        public SyncPodcastDbContext(DbContextOptions<SyncPodcastDbContext> options) : base(options) { } }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SyncPodcastDbContext).Assembly);
        }
    }
}
