using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens.Experimental;
using SyncPodcast.Domain.Entities;
using SyncPodcast.Domain.Interfaces;
using SyncPodcast.Infrastructure.Persistence;

namespace SyncPodcast.Infrastructure.Repositories;

public class PlaybackProgressRepository : IPlaybackProgressRepository
{
    private readonly SyncPodcastDbContext _db;
    public PlaybackProgressRepository(SyncPodcastDbContext db) => _db = db;

    public Task<PlaybackProgress?> GetAsync( Guid userId, Guid episodeId, CancellationToken ct)
    {
        return _db.PlaybackProgresses.FirstOrDefaultAsync(p => p.UserID == userId && p.EpisodeID == episodeId, ct);
    }

    public async Task<Dictionary<Guid, PlaybackProgress>> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
    /*returns a dictionary where the key is the episode ID and the value is
     * the playback progress for that episode for every episode the user has*/
        return await _db.PlaybackProgresses
            .Where(p => p.UserID == userId)
            .ToDictionaryAsync(p => p.EpisodeID, ct) ; 
    }

    public async Task SaveAsync(PlaybackProgress progress, CancellationToken ct)
    {
        PlaybackProgress? existingProgress = await _db.PlaybackProgresses
            .FirstOrDefaultAsync(p => p.ID == progress.ID);

        if (existingProgress == null)
        {
            _db.PlaybackProgresses.Add(progress);
        }
        else
        {
            _db.Update(progress);
        }
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid userId, Guid episodeId, CancellationToken ct)
    {
        PlaybackProgress? progress = await _db.PlaybackProgresses
            .FirstOrDefaultAsync(p => p.UserID == userId && p.EpisodeID == episodeId, ct);
        if (progress != null)
        {
            _db.PlaybackProgresses.Remove(progress);
            await _db.SaveChangesAsync(ct);
        }
    }

}
