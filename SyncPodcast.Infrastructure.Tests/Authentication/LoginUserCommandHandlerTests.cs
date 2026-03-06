using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using SyncPodcast.Application.CQRS;
using SyncPodcast.Domain.Entities;
using SyncPodcast.Domain.Interfaces;
using SyncPodcast.Domain.Tests.Helpers;
namespace SyncPodcast.Infrastructure.Tests.Authentication;

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
        User user = EntityFactory.CreateUser(passwordHash: "hashed");

        _userRepo.Setup(r => r.GetByUsernameAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _hashService.Setup(h => h.Verify("password123", "hashed")).Returns(true);

        _tokenService.Setup(t => t.GenerateToken(user.ID))
            .Returns(new AuthToken("access", "refresh",  
                DateTime.Now.AddHours(1), DateTime.Now.AddDays(7)));

        LoginUserCommand command = new LoginUserCommand("testuser", "password123");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("access");
        _userRepo.Verify(r => r.UpdateUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
