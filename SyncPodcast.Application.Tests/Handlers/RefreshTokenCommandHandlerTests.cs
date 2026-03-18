using Moq;
using FluentAssertions;
using Xunit;
using SyncPodcast.Application.CQRS;
using SyncPodcast.Domain.Interfaces;
using SyncPodcast.Domain.Exceptions;
using SyncPodcast.Domain.Entities;
using SyncPodcast.Domain.Tests.Helpers;

namespace SyncPodcast.Application.Tests.Handlers;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _handler = new RefreshTokenCommandHandler(_userRepo.Object, _tokenService.Object);
    }

    [Fact]
    public async Task Handle_ValidTokens_ReturnsNewAuthResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newToken = new AuthToken("new-access", "new-refresh", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddDays(7));
        _tokenService.Setup(t => t.RefreshToken("expired-access"))
            .Returns((userId, newToken));

        var user = EntityFactory.CreateUser(id: userId);
        user.SetRefreshToken("old-refresh", DateTime.UtcNow.AddDays(7));
        _userRepo.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new RefreshTokenCommand("expired-access", "old-refresh");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.AccessToken.Should().Be("new-access");
        result.RefreshToken.Should().Be("new-refresh");
        _userRepo.Verify(r => r.UpdateUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidAccessToken_ThrowsDomainException()
    {
        // Arrange
        _tokenService.Setup(t => t.RefreshToken("garbage"))
            .Returns(((Guid, AuthToken)?)null);

        var command = new RefreshTokenCommand("garbage", "some-refresh");

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("Invalid access token.");
    }
}
