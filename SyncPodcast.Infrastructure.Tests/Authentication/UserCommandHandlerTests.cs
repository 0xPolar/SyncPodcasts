using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using SyncPodcast.Application.CQRS;
using SyncPodcast.Domain.Entities;
using SyncPodcast.Domain.Exceptions;
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
        _userRepo.Verify(r => r.UpdateUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once); // makes sure the user's refresh token is updated in the repository
    }

    [Fact]
    public async Task Handle_IncorrectPassword_ThrowsDomainException()
    {
        User user = EntityFactory.CreateUser(passwordHash: "hashed");

        _userRepo.Setup(r => r.GetByUsernameAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        LoginUserCommand command = new LoginUserCommand("testuser", "wrongpassword");

        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsDomainException() 
    { 
        User user = EntityFactory.CreateUser(passwordHash: "hashed");

        _userRepo.Setup(r => r.GetByUsernameAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        LoginUserCommand command = new LoginUserCommand("fakeUser", "password123");


        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>();
    }
}
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
    public async Task Handle_RegisterUser_ReturnsToken()
    {
        User user = EntityFactory.CreateUser();

        _userRepo.Setup(r => r.AddAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _tokenService.Setup(t => t.GenerateToken(It.IsAny<Guid>()))
            .Returns(new AuthToken("access", "refresh",  
                DateTime.Now.AddHours(1), DateTime.Now.AddDays(7)));

        _hashService.Setup(h => h.Hash("password123")).Returns("hashed");

        RegisterUserCommand command = new RegisterUserCommand("testuser", "testuser@example.com", "hashed-password");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("access");
        _userRepo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);// makes sure the user is added to the repository
    }

    [Fact]
    public async Task Handle_ExistingUsername_ThrowsDomainException()
    {
        User user = EntityFactory.CreateUser();

        _userRepo.Setup(r => r.GetByUsernameAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        RegisterUserCommand command = new RegisterUserCommand("testuser", "testuser@example.com", "password123");

        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>();
    }

}
