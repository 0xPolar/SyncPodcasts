using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Infrastructure.Authentication
{
    public class JWTSettings 
    {
        public string Secret { get; init; }
        public string Issuer { get; init; }
        public string Audience { get; init; }
        public int ExpirationMinutes { get; init; }
        public int RefreshTokenExpirationDays { get; init; }

    }
} 