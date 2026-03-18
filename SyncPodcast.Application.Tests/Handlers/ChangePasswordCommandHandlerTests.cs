using Moq;
using FluentAssertions;
using Xunit;
using SyncPodcast.Application.CQRS;
using SyncPodcast.Domain.Interfaces;
using SyncPodcast.Domain.Exceptions;
using SyncPodcast.Domain.Entities;
using SyncPodcast.Domain.Tests.Helpers;

namespace SyncPodcast.Application.Tests.Handlers;

public class ChangePasswordCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IHashService> _hashService = new();
    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordCommandHandlerTests()
    {
        _handler = new ChangePasswordCommandHandler(_userRepo.Object, _hashService.Object);
    }

    [Fact]
    public async Task Handle_CorrectCurrentPassword_UpdatesHash()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = EntityFactory.CreateUser(id: userId, passwordHash: "old-hash");
        _userRepo.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _hashService.Setup(h => h.Verify("current-pw", "old-hash")).Returns(true);
        _hashService.Setup(h => h.Hash("new-pw")).Returns("new-hash");

        var command = new ChangePasswordCommand(userId, "current-pw", "new-pw");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        user.PasswordHash.Should().Be("new-hash");
        _userRepo.Verify(r => r.UpdateUserAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WrongCurrentPassword_ThrowsDomainException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = EntityFactory.CreateUser(id: userId, passwordHash: "old-hash");
        _userRepo.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _hashService.Setup(h => h.Verify("wrong-pw", "old-hash")).Returns(false);

        var command = new ChangePasswordCommand(userId, "wrong-pw", "new-pw");

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("Current password is incorrect.");
    }
}
