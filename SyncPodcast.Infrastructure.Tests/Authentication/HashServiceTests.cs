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
        string hash = _hashService.Hash("password123");

        bool verify = _hashService.Verify("password123", hash);

        Assert.True(verify);
    }

    [Fact]
    public void Verify_WithWrongPassword_ReturnsFalse()
    {
        string hash = _hashService.Hash("password123");

        bool verify = _hashService.Verify("wrongpassword", hash);

        Assert.False(verify);
    }

    [Fact]
    public void Hash_SameInputTwice_ProducesDifferentHashes()
    {
        string hash1 = _hashService.Hash("password123");
        string hash2 = _hashService.Hash("password123");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Hash_EmptyString_StillProducesHash()
    {
        string hash = _hashService.Hash("");

        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }
}

