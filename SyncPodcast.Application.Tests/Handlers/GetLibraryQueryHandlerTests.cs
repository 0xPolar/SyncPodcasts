using Moq;
using FluentAssertions;
using Xunit;
using SyncPodcast.Application.CQRS;
using SyncPodcast.Domain.Interfaces;
using SyncPodcast.Domain.Exceptions;
using SyncPodcast.Domain.Entities;
using SyncPodcast.Domain.Tests.Helpers;

namespace SyncPodcast.Application.Tests.Handlers;

public class GetLibraryQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<ISubscriptionRepository> _subRepo = new();
    private readonly GetLibraryQueryHandler _handler;

    public GetLibraryQueryHandlerTests()
    {
        _handler = new GetLibraryQueryHandler(_userRepo.Object, _subRepo.Object);
    }

    [Fact]
    public async Task Handle_WithSubscribedPodcasts_ReturnsDTOs()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = EntityFactory.CreateUser(id: userId);
        var podcast = EntityFactory.CreatePodcast(title: "My Podcast");

        _userRepo.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _subRepo.Setup(r => r.GetUserPodcastsAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Podcast> { podcast });

        // Act
        var result = await _handler.Handle(new GetLibraryQuery(userId), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("My Podcast");
        result.First().PodcastId.Should().Be(podcast.ID);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsDomainException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepo.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(new GetLibraryQuery(userId), CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage($"User with ID {userId} not found.");
    }

    [Fact]
    public async Task Handle_NoPodcasts_ThrowsDomainException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = EntityFactory.CreateUser(id: userId);
        _userRepo.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _subRepo.Setup(r => r.GetUserPodcastsAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<Podcast>?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(new GetLibraryQuery(userId), CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage($"No podcasts found for user with ID {userId}.");
    }
}
