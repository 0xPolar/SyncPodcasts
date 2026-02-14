using SyncPodcast.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPoscast.Infrastructure.Authentication;

public class HashService : IHashService
{
    public string Hash(string password)
    {
        // Implement a secure hashing algorithm, e.g., BCrypt or Argon2
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    public bool Verify(string password, string hashedPassword)
    {
        // Verify the password against the hashed version
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
