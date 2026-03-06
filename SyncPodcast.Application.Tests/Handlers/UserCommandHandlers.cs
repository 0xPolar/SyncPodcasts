using Moq;
using FluentAssertions;
using Xunit;
using SyncPodcast.Application.CQRS;
using SyncPodcast.Domain.Interfaces;

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
    public async Task Handle_WIthValidCredentials_ReturnsAuthResults()
    {
        var user = EntityFactory.CreateUser();
    }
}
