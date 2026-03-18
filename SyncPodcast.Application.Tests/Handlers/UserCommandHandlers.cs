using Moq;
using FluentAssertions;
using Xunit;
using SyncPodcast.Application.CQRS;
using SyncPodcast.Domain.Interfaces;
using SyncPodcast.Domain.Exceptions;
using SyncPodcast.Domain.Tests.Helpers;

namespace SyncPodcast.Application.Tests.Handlers;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly Mock<IHashService> _hashService = new();
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _handler = new LoginUserCommandHandler(_userRepo.Object, _tokenService.Object, _hashService.Object);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsAuthResult()
    {
        // Arrange
        var user = EntityFactory.CreateUser(username: "testuser", passwordHash: "hashed");
        _userRepo.Setup(r => r.GetByUsernameAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _hashService.Setup(h => h.Verify("password123", "hashed")).Returns(true);
        _tokenService.Setup(t => t.GenerateToken(user.ID))
            .Returns(new AuthToken("access", "refresh", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddDays(7)));

        var command = new LoginUserCommand("testuser", "password123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(user.ID);
        result.Username.Should().Be("testuser");
        result.AccessToken.Should().Be("access");
        result.RefreshToken.Should().Be("refresh");
        _userRepo.Verify(r => r.UpdateUserAsync(It.IsAny<Domain.Entities.User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsDomainException()
    {
        // Arrange
        _userRepo.Setup(r => r.GetByUsernameAsync("nobody", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.User?)null);

        var command = new LoginUserCommand("nobody", "password123");

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("Invalid username or password.");
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsDomainException()
    {
        // Arrange
        var user = EntityFactory.CreateUser(username: "testuser", passwordHash: "hashed");
        _userRepo.Setup(r => r.GetByUsernameAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _hashService.Setup(h => h.Verify("wrong", "hashed")).Returns(false);

        var command = new LoginUserCommand("testuser", "wrong");

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("Invalid username or password.");
    }
}
