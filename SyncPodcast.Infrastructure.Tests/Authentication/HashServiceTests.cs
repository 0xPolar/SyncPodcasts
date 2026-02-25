using SyncPodcast.Infrastructure.Authentication;

namespace SyncPodcast.Infrastructure.Tests.Authentication;

public class HashServiceTests
{
    private readonly HashService _hashService = new();

    [Fact]
    public void Hash_ReturnsNonNull()
    {
        var hash = _hashService.Hash("password123");
        Assert.NotNull(hash);
    }
}

