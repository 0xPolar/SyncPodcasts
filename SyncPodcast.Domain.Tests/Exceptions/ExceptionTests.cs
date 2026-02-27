using SyncPodcast.Domain.Exceptions;

namespace SyncPodcast.Domain.Tests.Exceptions;

public class ExceptionTests
{
    [Fact]
    public void NotFoundException_FormatsMessage_WithEntityAndKey()
    {
        var key = Guid.NewGuid();

        var ex = new NotFoundException("User", key);

        Assert.Contains("User", ex.Message);
        Assert.Contains(key.ToString(), ex.Message);
    }

    [Fact]
    public void DomainException_SetsMessage()
    {
        var ex = new DomainException("something broke");

        Assert.Equal("something broke", ex.Message);
    }
}
