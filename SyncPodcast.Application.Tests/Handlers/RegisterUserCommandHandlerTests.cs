using Moq;
using FluentAssertions;
using Xunit;
using SyncPodcast.Application.CQRS;
using SyncPodcast.Domain.Interfaces;
using SyncPodcast.Domain.Exceptions;
using SyncPodcast.Domain.Entities;
using SyncPodcast.Domain.Tests.Helpers;

namespace SyncPodcast.Application.Tests.Handlers;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly Mock<IHashService> _hashService = new();
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _handler = new RegisterUserCommandHandler(_userRepo.Object, _tokenService.Object, _hashService.Object);
    }

    [Fact]
    public async Task Handle_NewUser_CreatesAndReturnsToken()
    {
        // Arrange
        _userRepo.Setup(r => r.GetByUsernameAsync("newuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _hashService.Setup(h => h.Hash("password123")).Returns("hashed-pw");
        _tokenService.Setup(t => t.GenerateToken(It.IsAny<Guid>()))
            .Returns(new AuthToken("access", "refresh", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddDays(7)));

        var command = new RegisterUserCommand("newuser", "new@test.com", "password123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be("newuser");
        result.AccessToken.Should().Be("access");
        result.RefreshToken.Should().Be("refresh");
        _userRepo.Verify(r => r.AddAsync(It.Is<User>(u => u.Username == "newuser"), It.IsAny<CancellationToken>()), Times.Once);
        _userRepo.Verify(r => r.UpdateUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ExistingUsername_ThrowsDomainException()
    {
        // Arrange
        var existingUser = EntityFactory.CreateUser(username: "taken");
        _userRepo.Setup(r => r.GetByUsernameAsync("taken", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        var command = new RegisterUserCommand("taken", "new@test.com", "password123");

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("Username is already taken.");
    }
}
