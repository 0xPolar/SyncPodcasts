using SyncPodcast.Infrastructure.Authentication;
using SyncPodcast.Domain.Interfaces;

namespace SyncPodcast.Infrastructure.Tests.Authentication;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;

    private readonly JWTSettings _jwtSettings = new JWTSettings
    {
        Secret = "this-is-a-test-secret-key-that-is-long-enough-for-hmac-sha256!",
        Issuer = "test-issuer",
        Audience = "test-audience",
        ExpirationMinutes = 15,
        RefreshTokenExpirationDays = 7
    };

    public TokenServiceTests()
    {
        _tokenService = new TokenService(_jwtSettings);

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

    [Fact]
    public void ValidateToken_WithWrongSecret_ReturnsNull()
    {
        Guid userId = Guid.NewGuid();

        AuthToken token = _tokenService.GenerateToken(userId);

        JWTSettings altJwtSettings = _jwtSettings with { Secret = "" };
        TokenService altTokenService = new TokenService(altJwtSettings);

        Guid? tokenUserID = altTokenService.ValidateToken(token.AccessToken);

        Assert.NotEqual(tokenUserID, userId);


    }

    [Fact]
    public void RefreshToken_WithValidExpiredToken_ReturnsNewTokenAndUserId()
    {
        Guid userId = Guid.NewGuid();
        JWTSettings shortExpirySettings = _jwtSettings with { ExpirationMinutes = -1 };

        TokenService shortExpiryTokenService = new TokenService(shortExpirySettings);
        var token = shortExpiryTokenService.GenerateToken(userId);

        var tokenSet = _tokenService.RefreshToken(token.AccessToken);
        Assert.NotNull(tokenSet);

        var (newUserId, newToken) = tokenSet.Value;

        Assert.Equal(userId, newUserId);
        Assert.True(newToken.ExpiresAt > DateTime.UtcNow);
    }
}
