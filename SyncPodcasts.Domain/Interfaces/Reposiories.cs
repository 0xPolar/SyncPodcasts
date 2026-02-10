using SyncPodcast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username, CancellationToken ct);
        Task<User?> GetByEmailAsync(string email, CancellationToken ct);
        Task<User?> GetByIdAsync(Guid userId, CancellationToken ct);
        Task AddAsync(User user, CancellationToken ct);
        Task<User?> UpdateUser(User user, CancellationToken ct);
         Task DeleteAsync(Guid userId, CancellationToken ct);
    }
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

    public record PodcastSearchResult(string Title, string Author, Uri FeedUrl, Uri? ArtworkUrl);

    public interface ITokenService
    {
        AuthTokens GenerateToken(Guid userId);
        Guid? ValidateToken(string token);
    }

    public record AuthTokens(string AccessToken, string RefreshToken, DateTime ExpiresAt);

}
