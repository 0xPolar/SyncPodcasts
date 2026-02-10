using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Application.CQRS
{
    public record AuthUserResultDTO(Guid UserId, string Username, string AccessToken, string RefreshToken, DateTime ExpiresAt);
    public record SubscibeResultDTO(Guid PodcastId, string Title, string Author, Uri FeedUrl, Uri? ArtworkUrl);
    public record LibraryDTO(Guid PodcastId, string Title, string Author, Uri FeedUrl, Uri? ArtworkUrl);
    public record PodcastSearchResultDTO(Guid PodcastId, string Title, string Author, Uri FeedUrl, Uri? ArtworkUrl);
    public record PodcastDetailsDTO(Guid PodcastId, string Title, string Author, Uri FeedUrl, Uri? ArtworkUrl, List<EpisodeDTO> Episodes);
    public record EpisodeDTO(Guid EpisodeId, string Title, TimeSpan Duration, DateTime PublishDate, Uri MediaUrl);
    public record PlaybackProgressDTO(Guid EpisodeId, TimeSpan Progress);

}
