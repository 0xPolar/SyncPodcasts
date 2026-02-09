using SyncPodcast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Domain.Interfaces
{
    public interface IPoscastRepository
    {
        Task<Podcast?> GetPodcastByIdAsync(Guid podcastId, CancellationToken ct);
        Task<Podcast?> GetByFeedUrlAsync(Uri feedUrl, CancellationToken ct);
        Task AddAsync(Podcast podcast, CancellationToken ct);
        Task UpdateAsync(Podcast podcast, CancellationToken ct);
         Task DeleteAsync(Guid podcastId, CancellationToken ct);
    }

    public interface ISubscriptionRepository
    {
        Task<bool> ExistsAsync(Guid userId, Guid podcastId, CancellationToken ct);
        Task<List<Podcast>> GetUserPodcastsAsync(Guid userId, CancellationToken ct);
        Task AddAsync(Subscription subscription, CancellationToken ct);
        Task DeleteAsync(Guid userId, Guid podcastId, CancellationToken ct);
    }

    public interface  IPlaybackProgressRepository 
    {
        Task<PlaybackProgress?> GetAsync(Guid userId, Guid episodeId, CancellationToken ct);
        Task<Dictionary<Guid, PlaybackProgress>> GetByUserIdAsync(Guid userId, CancellationToken ct);
        Task SaveAsync(PlaybackProgress progress, CancellationToken ct);
         Task DeleteAsync(Guid userId, Guid episodeId, CancellationToken ct);
    }

    public interface IRssParser
    {
               Task<Podcast> ParseAsync(Uri feedUrl, CancellationToken ct);
    }

    public interface IPodcastSearchService
    {
        Task<List<PodcastSearchResult>> SearchAsync(string query, CancellationToken ct);
    }

    public class PodcastSearchResult
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public Uri FeedUrl { get; set; }
        public Uri? ArtworkUrl { get; set; }

    }

    public interface ITokenService
    {
        AuthTokens GenerateToken(Guid userId);
        Guid? ValidateToken(string token);
    }

    public record AuthTokens(string AccessToken, string RefreshToken, DateTime ExpiresAt);

}
