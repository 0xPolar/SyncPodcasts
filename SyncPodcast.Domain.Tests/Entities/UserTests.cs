using SyncPodcast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Domain.Tests.Entities;

public class  UserTests
{
    [Fact]
    public void Constructor_WithValidArgs_SetsallProperties()
    {
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var user = new User(id, "john21", "john@example.com", "hash123", now);

        Assert.Equal(id, user.ID);
        Assert.Equal("alice", user.Username);
        Assert.Equal("alice@test.com", user.Email);
        Assert.Equal("hash123", user.PasswordHash);
        Assert.Equal(now, user.CreatedAt);
        Assert.Null(user.RefreshToken);
        Assert.Null(user.RefreshTokenExpiry);
    }
    
}
