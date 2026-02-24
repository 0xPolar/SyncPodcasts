using System.Xml.Linq;
using SyncPodcast.Domain.Entities;
using SyncPodcast.Domain.Interfaces;

namespace SyncPodcast.Infrastructure.Services;

public class RssParser : IRssParser
{
    private static readonly XNamespace Itunes = "http://www.itunes.com/dtds/podcast-1.0.dtd";

    private readonly IHttpClientFactory _httpClientFactory;

    public RssParser(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Podcast> ParseAsync(Uri feedUrl, CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient();
        var xml = await client.GetStringAsync(feedUrl, ct);
        var doc = XDocument.Parse(xml);

        var channel = doc.Root!.Element("channel")
            ?? throw new InvalidOperationException("RSS feed is missing <channel> element.");

        var title = channel.Element("title")?.Value ?? string.Empty;
        var author = channel.Element(Itunes + "author")?.Value ?? string.Empty;
        var description = channel.Element("description")?.Value ?? string.Empty;
        var artworkUrl = channel.Element(Itunes + "image")?.Attribute("href")?.Value;

        var podcast = new Podcast(
            title,
            author,
            description,
            feedUrl,
            artworkUrl != null ? new Uri(artworkUrl) : null
        );

        foreach (var item in channel.Elements("item"))
        {
            var epTitle = item.Element("title")?.Value ?? string.Empty;
            var epDescription = item.Element("description")?.Value ?? string.Empty;
            var enclosure = item.Element("enclosure");
            var mediaUrl = enclosure?.Attribute("url")?.Value;
            var durationStr = item.Element(Itunes + "duration")?.Value;
            var pubDateStr = item.Element("pubDate")?.Value;

            if (mediaUrl == null) continue;

            var duration = ParseDuration(durationStr);
            var publishedAt = DateTime.TryParse(pubDateStr, out var d) ? d : DateTime.UtcNow;

            var episode = new Episode(
                podcast.ID,
                epTitle,
                epDescription,
                new Uri(mediaUrl),
                duration,
                publishedAt
            );

            podcast.AddEpisode(episode);
        }

        return podcast;
    }

    private static TimeSpan ParseDuration(string? duration)
    {
        if (string.IsNullOrWhiteSpace(duration))
            return TimeSpan.Zero;

        if (int.TryParse(duration, out var totalSeconds))
            return TimeSpan.FromSeconds(totalSeconds);

        var parts = duration.Split(':');
        return parts.Length switch
        {
            3 => new TimeSpan(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])),
            2 => new TimeSpan(0, int.Parse(parts[0]), int.Parse(parts[1])),
            _ => TimeSpan.Zero
        };
    }
}
