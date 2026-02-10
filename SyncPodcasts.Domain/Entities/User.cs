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

        public User(Guid id, string username, string email, string passwordHash, DateTime createdAt)
        {
            ID = id;
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            CreatedAt = createdAt;
        }
        private User() { }
    }
}
