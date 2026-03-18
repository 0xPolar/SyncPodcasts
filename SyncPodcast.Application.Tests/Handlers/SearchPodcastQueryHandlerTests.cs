using Moq;
using FluentAssertions;
using Xunit;
using SyncPodcast.Application.CQRS;
using SyncPodcast.Domain.Interfaces;

namespace SyncPodcast.Application.Tests.Handlers;

public class SearchPodcastQueryHandlerTests
{
    private readonly Mock<IPodcastSearchService> _searchService = new();
    private readonly SearchPodcastQueryHandler _handler;

    public SearchPodcastQueryHandlerTests()
    {
        _handler = new SearchPodcastQueryHandler(_searchService.Object);
    }

    [Fact]
    public async Task Handle_DelegatesToSearchServiceAndMapsDTOs()
    {
        // Arrange
        var searchResults = new List<PodcastSearchResult>
        {
            new(Guid.NewGuid(), "Pod 1", "Author 1", new Uri("https://example.com/feed1.xml"), null),
            new(Guid.NewGuid(), "Pod 2", "Author 2", new Uri("https://example.com/feed2.xml"), new Uri("https://example.com/art.jpg"))
        };
        _searchService.Setup(s => s.SearchAsync("test", 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(searchResults);

        var query = new SearchPodcastQuery("test", 1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Title.Should().Be("Pod 1");
        result[1].Title.Should().Be("Pod 2");
        result[1].ArtworkUrl.Should().NotBeNull();
        _searchService.Verify(s => s.SearchAsync("test", 1, 10, It.IsAny<CancellationToken>()), Times.Once);
    }
}
