using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using SyncPodcast.Domain.Entities;
using SyncPodcast.Domain.Interfaces;
using SyncPodcast.Infrastructure.Persistence;

namespace SyncPodcast.Infrastructure.Repositories;

public class PodcastRepository : IPodcastRepository
{
    private readonly SyncPodcastDbContext _db;
    public PodcastRepository(SyncPodcastDbContext db) => _db = db;

    public async Task<Podcast?> GetPodcastByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Podcasts
            .Include(p => p.Episodes) // Include episodes when fetching the podcast
            .FirstOrDefaultAsync(p => p.ID == id, ct);
    }

    public async Task<Podcast?> GetByFeedUrlAsync(Uri feedUrl, CancellationToken ct)
    {
        // used when checking if podcast with url exists no need to include episodes
        return await _db.Podcasts
            .FirstOrDefaultAsync(p => p.FeedUrl == feedUrl, ct);
    }

    public async Task<Episode?> GetEpisodeByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Episodes
            .FirstOrDefaultAsync(e => e.ID == id, ct);
    }

    public async Task AddAsync(Podcast podcast, CancellationToken ct)
    {
        _db.Add(podcast);

        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Podcast podcast, CancellationToken ct)
    {
        _db.Update(podcast);

        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        Podcast? podcast = await _db.Podcasts.FirstOrDefaultAsync(p => p.ID == id, ct);
        if (podcast is not null)
        {
            _db.Remove(podcast);

            await _db.SaveChangesAsync(ct);
        }
    }
}
