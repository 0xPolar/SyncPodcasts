using SyncPodcast.Domain.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace SyncPodcast.Infrastructure.Services;

public class PodcastSearchService : IPodcastSearchService
{
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public PodcastSearchService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<PodcastSearchResult>> SearchAsync(string query, int page, int pageSize, CancellationToken ct)
    {
        var encodedQuery = HttpUtility.UrlEncode(query);
        var offset = (page - 1) * pageSize;
        var url = $"https://itunes.apple.com/search?term={encodedQuery}&entity=podcast&limit={pageSize}&offset={offset}";

        var client = _httpClientFactory.CreateClient();
        var responseString = await client.GetStringAsync(url, ct);

        var response = JsonSerializer.Deserialize<ITunesResponse>(responseString, JsonOptions);

        if (response?.Results is null)
            return [];

        return response.Results
            .Select(r => new PodcastSearchResult(
                Guid.NewGuid(),
                r.CollectionName ?? string.Empty,
                r.ArtistName ?? string.Empty,
                r.FeedUrl is not null ? new Uri(r.FeedUrl) : null!,
                r.ArtworkUrl600 is not null ? new Uri(r.ArtworkUrl600) : null
            ))
            .Where(r => r.FeedUrl is not null)
            .ToList();
    }

    private sealed class ITunesResponse
    {
        public int ResultCount { get; set; }
        public List<ITunesResult> Results { get; set; } = [];
    }

    private sealed class ITunesResult
    {
        public string? CollectionName { get; set; }
        public string? ArtistName { get; set; }
        public string? FeedUrl { get; set; }
        [JsonPropertyName("artworkUrl600")]
        public string? ArtworkUrl600 { get; set; }
    }
}
