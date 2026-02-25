using SyncPodcast.Infrastructure.Authentication;

namespace SyncPodcast.Infrastructure.Tests.Authentication;

public class HashServiceTests
{
    private readonly HashService _hashService = new();

    [Fact]
    public void Hash_ReturnsNonNull()
    {
        string hash = _hashService.Hash("password123");
        Assert.NotNull(hash);
    }

    [Fact]
    public void Verify_WithCorrectPassword_ReturnsTrue()
    {
        var hash = _hashService.Hash("password123");

        bool verify = _hashService.Verify("password123", hash);

        Assert.True(verify);

    }
}

