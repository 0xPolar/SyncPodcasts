using SyncPodcast.Infrastructure.Authentication;
using SyncPodcast.Domain.Interfaces;

namespace SyncPodcast.Infrastructure.Tests.Authentication;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        var settings = new JWTSettings
        {
            Secret = "this-is-a-test-secret-key-that-is-long-enough-for-hmac-sha256!",
            Issuer = "test-issuer",
            Audience = "test-audience",
            ExpirationMinutes = 15,
            RefreshTokenExpirationDays = 7
        };
        _tokenService = new TokenService(settings);

    }

    [Fact]
    public void GenerateToken_ReturnsValidAuthToken()
    {
        Guid userId = Guid.NewGuid();

        AuthToken token = _tokenService.GenerateToken(userId);

        Assert.NotNull(token.AccessToken);
        Assert.NotNull(token.RefreshToken);
        Assert.True(token.ExpiresAt > DateTime.UtcNow);
        Assert.True(token.RefreshTokenExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public void ValidateToken_WithValidToken_ReturnsUserId()
    {
        Guid userId = Guid.NewGuid();

        AuthToken token = _tokenService.GenerateToken(userId);

        Guid? tokenUserID = _tokenService.ValidateToken(token.AccessToken);

        Assert.Equal(tokenUserID, userId);
    }

    [Fact]
    public void ValidateToken_WithGarbageString_ReturnsNull()
    {
        Guid? tokenUserID = _tokenService.ValidateToken("this-is-not-a-valid-token");


        Assert.Null(tokenUserID);
    }
}
