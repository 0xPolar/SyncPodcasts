using SyncPodcast.Domain.Entities;
using SyncPodcast.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Domain.Tests.Entities;

public class  UserTests
{
    [Fact]
    public void Constructor_WithValidArgs_SetsallProperties()
    {
        Guid id = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;

        User user = new User(id, "john21", "john@example.com", "hash123", now);

        Assert.Equal(id, user.ID);
        Assert.Equal("john21", user.Username);
        Assert.Equal("john@example.com", user.Email);
        Assert.Equal("hash123", user.PasswordHash);
        Assert.Equal(now, user.CreatedAt);
        Assert.Null(user.RefreshToken);
        Assert.Null(user.RefreshTokenExpiry);
    }

    [Fact]
    public void SetRefreshToken_WithValidArgs_SetsTokenAndExpiry()
    {
        Guid id = Guid.NewGuid();
        string token = Guid.NewGuid().ToString();
        DateTime now = DateTime.UtcNow;

        User user = new User(id, "john21", "john@example.com", "hash123", now);

        user.SetRefreshToken(token, now);

        Assert.Equal(token, user.RefreshToken);
        Assert.Equal(now, user.RefreshTokenExpiry);
    }

    [Fact]
    public void ValidateRefreshToken_WhenTokenMatches_DoesNotThrow()
    {
        Guid id = Guid.NewGuid();
        string token = Guid.NewGuid().ToString();
        DateTime now = DateTime.UtcNow.AddDays(1);

        User user = new User(id, "john21", "john@example.com", "hash123", now);

        user.SetRefreshToken(token, now);

        user.ValidateRefreshToken(token);
    }

    [Fact]
    public void ValidateRefreshToken_WhenTokenMatches_ThrowsDomainException()
    {
        Guid id = Guid.NewGuid();
        string token = Guid.NewGuid().ToString();
        DateTime now = DateTime.UtcNow;

        User user = new User(id, "john21", "john@example.com", "hash123", now);

        user.SetRefreshToken(token, now);


        Assert.Throws<DomainException>(() => user.ValidateRefreshToken("wrong-token"));
    }

    [Fact]
    public void ValidateRefreshToken_WhenTokenExpired_ThrowsDomainException()
    {
        Guid id = Guid.NewGuid();
        string token = Guid.NewGuid().ToString();
        DateTime now = DateTime.UtcNow.AddDays(-1);

        User user = new User(id, "john21", "john@example.com", "hash123", now);

        user.SetRefreshToken(token, now);


        Assert.Throws<DomainException>(() => user.ValidateRefreshToken(token));
    }

    [Fact]
    public void ValidateRefreshToken_WhenExpiryIsNull_ThrowsDomainException()
    {
        Guid id = Guid.NewGuid();
        string token = Guid.NewGuid().ToString();
        DateTime now = DateTime.UtcNow.AddDays(-1);

        User user = new User(id, "john21", "john@example.com", "hash123", now);


        Assert.Throws<DomainException>(() => user.ValidateRefreshToken(token));
    }

    [Fact]
    public void ClearRefreshToken_AfterSettingToken_SetsFieldsToNull()
    {
        Guid id = Guid.NewGuid();
        string token = Guid.NewGuid().ToString();
        DateTime now = DateTime.UtcNow.AddDays(1);

        User user = new User(id, "john21", "john@example.com", "hash123", now);

        user.SetRefreshToken(token, now);

        user.ClearRefreshToken();

        Assert.Equal(user.RefreshToken, null);
        Assert.Equal(user.RefreshTokenExpiry, null);
    }

    [Fact]
    public void ChangePassword_WithNewHash_UpdatesHashAndClearsRefreshToken()
    {
        Guid id = Guid.NewGuid();
        string token = Guid.NewGuid().ToString();
        string password = "password";
        DateTime now = DateTime.UtcNow.AddDays(1);

        User user = new User(id, "john21", "john@example.com", password, now);

        user.SetRefreshToken(token, now);

        user.ChangePassword("new-password");

        Assert.NotEqual(user.RefreshToken, token);
        Assert.NotEqual(user.RefreshTokenExpiry, now);
    }

}
