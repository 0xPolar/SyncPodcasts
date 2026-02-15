using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Domain.Entities
{
    public class User
    {
        public Guid ID { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiry { get; private set; }

        public User(Guid id, string username, string email, string passwordHash, DateTime createdAt)
        {
            ID = id;
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            CreatedAt = createdAt;
        }

        public void ValidateRefreshToken(string refreshToken)
        {
            if (RefreshToken != refreshToken || RefreshTokenExpiry <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        public void SetRefreshToken(string refreshToken, DateTime expiry)
        {
            RefreshToken = refreshToken;
            RefreshTokenExpiry = expiry;
        }

        public void ClearRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiry = null;
        }

        private User() { }
    }
}
