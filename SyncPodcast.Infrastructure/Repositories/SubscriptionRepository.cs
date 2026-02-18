using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using SyncPodcast.Domain.Entities;
using SyncPodcast.Domain.Interfaces;
using SyncPodcast.Infrastructure.Persistence;

namespace SyncPodcast.Infrastructure.Repositories;


public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly SyncPodcastDbContext _db;
    public SubscriptionRepository(SyncPodcastDbContext db) => _db = db;

    public async Task<bool> ExistsAsync(Guid userId, Guid podcastId, CancellationToken ct)
    {
        return await _db.Subscriptions
            .AnyAsync(s => s.UserID == userId && s.PodcastID == podcastId, ct);
    }

    public async Task<List<Podcast>> GetUserPodcastsAsync(Guid userId, CancellationToken ct)
    {
        return await _db.Subscriptions
            .Where(s => s.UserID == userId)
            .Join(_db.Podcasts,
                s => s.PodcastID,
                p => p.ID,
                (s, p) => p)
            .Include(p => p.Episodes)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Subscription subscription, CancellationToken ct)
    {
        await _db.Subscriptions.AddAsync(subscription, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid userId, Guid podcastId, CancellationToken ct)
        {
            var subscription = await _db.Subscriptions
                .FirstOrDefaultAsync(s => s.UserID == userId && s.PodcastID == podcastId, ct);
    
            if (subscription != null)
            {
                _db.Subscriptions.Remove(subscription);
                await _db.SaveChangesAsync(ct);
            }
    }
}
