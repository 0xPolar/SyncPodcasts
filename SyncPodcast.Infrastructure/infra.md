# Infrastructure Layer — Implementation TODO

Your current directories: `Authentication/`, `Configurations/`, `Migrations/`, `Services/`
You'll also need to create: `Persistence/`, `Repositories/`

---

## Step 0: Add NuGet Packages

**File:** `SyncPoscast.Infrastructure.csproj`

Before anything compiles, you need these packages:

```
dotnet add package Microsoft.EntityFrameworkCore --version 10.0.0-*
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 10.0.0-*
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 10.0.0-*
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 10.0.0-*
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
dotnet add package BCrypt.Net-Next
```

**Why:** EF Core for database access, Npgsql for PostgreSQL provider, JWT packages for token generation/validation, BCrypt for password hashing, Redis for caching.

---

## Step 1: EF Core DbContext

**File:** `Persistence/SyncPodcastDbContext.cs`
**Directory:** Create `Persistence/`

**What it does:**
- Inherits from `DbContext`
- Defines `DbSet<T>` properties for: `User`, `Podcast`, `Episode`, `Subscription`, `PlaybackProgress`
- Overrides `OnModelCreating` to call `builder.ApplyConfigurationsFromAssembly(typeof(SyncPodcastDbContext).Assembly)` — this auto-discovers all your entity configurations
- This is the single gateway between your app and PostgreSQL

**Depends on:** Domain entities, NuGet packages (Step 0)

---

## Step 2: Entity Configurations (Fluent API)

**Directory:** `Configurations/` (already exists)

These tell EF Core how to map your domain entities to database tables. One file per entity.

### 2a — `Configurations/UserConfiguration.cs`

**Implements:** `IEntityTypeConfiguration<User>`

| What to configure | Why |
|---|---|
| `HasKey(u => u.ID)` | Primary key |
| `Property(u => u.Username).HasMaxLength(100).IsRequired()` | Column constraints |
| `HasIndex(u => u.Username).IsUnique()` | No duplicate usernames |
| `HasIndex(u => u.Email).IsUnique()` | No duplicate emails |
| `Property(u => u.Email).HasMaxLength(255).IsRequired()` | Column constraints |
| `Property(u => u.PasswordHash).IsRequired()` | Can't be null |

### 2b — `Configurations/PodcastConfiguration.cs`

**Implements:** `IEntityTypeConfiguration<Podcast>`

| What to configure | Why |
|---|---|
| `HasKey(p => p.ID)` | Primary key |
| `Property(p => p.Title).HasMaxLength(500).IsRequired()` | Column constraints |
| `HasIndex(p => p.FeedUrl).IsUnique()` | One record per feed URL |
| `HasMany(p => p.Episodes).WithOne().HasForeignKey(e => e.PodcastID).OnDelete(DeleteBehavior.Cascade)` | Parent-child relationship |
| `Property(p => p.FeedUrl).HasConversion(...)` | Uri to string conversion |
| `Property(p => p.ArtworkUrl).HasConversion(...)` | Uri? to string? conversion |
| Access backing field `_episodes` via `builder.Metadata.FindNavigation(nameof(Podcast.Episodes)).SetPropertyAccessMode(PropertyAccessMode.Field)` | EF Core populates the private `_episodes` list |

### 2c — `Configurations/EpisodeConfiguration.cs`

**Implements:** `IEntityTypeConfiguration<Episode>`

| What to configure | Why |
|---|---|
| `HasKey(e => e.ID)` | Primary key |
| `Property(e => e.Title).HasMaxLength(500).IsRequired()` | Column constraints |
| `Property(e => e.MediaUrl).HasConversion(...)` | Uri to string conversion |
| `HasIndex(e => e.PodcastID)` | FK index for query performance |

### 2d — `Configurations/SubscriptionConfiguration.cs`

**Implements:** `IEntityTypeConfiguration<Subscription>`

| What to configure | Why |
|---|---|
| `HasKey(s => s.ID)` | Primary key |
| `HasIndex(s => new { s.UserID, s.PodcastID }).IsUnique()` | Composite unique — user can't subscribe twice |
| `HasIndex(s => s.UserID)` | FK index for "get my library" queries |

### 2e — `Configurations/PlaybackProgressConfiguration.cs`

**Implements:** `IEntityTypeConfiguration<PlaybackProgress>`

| What to configure | Why |
|---|---|
| `HasKey(p => p.ID)` | Primary key |
| `HasIndex(p => new { p.UserID, p.EpisodeID }).IsUnique()` | One progress record per user+episode |
| `HasIndex(p => p.UserID)` | FK index for "get all progress" queries |

---

## Step 3: Repository Implementations

**Directory:** Create `Repositories/`

Each repository implements a Domain interface using the DbContext. They are thin wrappers around EF Core queries.

### 3a — `Repositories/UserRepository.cs`

**Implements:** `IUserRepository`
**Injects:** `SyncPodcastDbContext`

| Method | Implementation |
|---|---|
| `GetByUsernameAsync` | `_db.Users.FirstOrDefaultAsync(u => u.Username == username, ct)` |
| `GetByEmailAsync` | `_db.Users.FirstOrDefaultAsync(u => u.Email == email, ct)` |
| `GetByIdAsync` | `_db.Users.FirstOrDefaultAsync(u => u.ID == userId, ct)` |
| `AddAsync` | `_db.Users.Add(user); await _db.SaveChangesAsync(ct)` |
| `UpdateUser` | `_db.Users.Update(user); await _db.SaveChangesAsync(ct); return user` |
| `DeleteAsync` | Find by ID, then `_db.Users.Remove(user); await _db.SaveChangesAsync(ct)` |

### 3b — `Repositories/PodcastRepository.cs`

**Implements:** `IPodcastRepository`
**Injects:** `SyncPodcastDbContext`

| Method | Implementation |
|---|---|
| `GetPodcastByIdAsync` | `_db.Podcasts.Include(p => p.Episodes).FirstOrDefaultAsync(p => p.ID == podcastId, ct)` — include episodes so handlers can access them |
| `GetByFeedUrlAsync` | `_db.Podcasts.FirstOrDefaultAsync(p => p.FeedUrl == feedUrl, ct)` |
| `GetEpisodeByIdAsync` | `_db.Episodes.FirstOrDefaultAsync(e => e.ID == episodeId, ct)` — note: queries Episodes DbSet directly |
| `AddAsync` | `_db.Podcasts.Add(podcast); await _db.SaveChangesAsync(ct)` |
| `UpdateAsync` | `_db.Podcasts.Update(podcast); await _db.SaveChangesAsync(ct)` |
| `DeleteAsync` | Find + Remove + SaveChanges |

### 3c — `Repositories/SubscriptionRepository.cs`

**Implements:** `ISubscriptionRepository`
**Injects:** `SyncPodcastDbContext`

| Method | Implementation |
|---|---|
| `ExistsAsync` | `_db.Subscriptions.AnyAsync(s => s.UserID == userId && s.PodcastID == podcastId, ct)` |
| `GetUserPodcastsAsync` | Join subscriptions to podcasts: `_db.Subscriptions.Where(s => s.UserID == userId).Join(_db.Podcasts, s => s.PodcastID, p => p.ID, (s, p) => p).ToListAsync(ct)` |
| `AddAsync` | `_db.Subscriptions.Add(subscription); await _db.SaveChangesAsync(ct)` |
| `DeleteAsync` | Find by UserID+PodcastID, Remove, SaveChanges |

### 3d — `Repositories/PlaybackProgressRepository.cs`

**Implements:** `IPlaybackProgressRepository`
**Injects:** `SyncPodcastDbContext`

| Method | Implementation |
|---|---|
| `GetAsync` | `_db.PlaybackProgress.FirstOrDefaultAsync(p => p.UserID == userId && p.EpisodeID == episodeId, ct)` |
| `GetByUserIdAsync` | `_db.PlaybackProgress.Where(p => p.UserID == userId).ToDictionaryAsync(p => p.EpisodeID, ct)` |
| `SaveAsync` | Upsert pattern — check if exists, if yes `Update()`, if no `Add()`, then `SaveChangesAsync()` |
| `DeleteAsync` | Find by UserID+EpisodeID, Remove, SaveChanges |

---

## Step 4: Authentication Services

**Directory:** `Authentication/` (already exists, has `JWTSettings.cs`)

### 4a — `Authentication/TokenService.cs`

**Implements:** `ITokenService`
**Injects:** `IOptions<JWTSettings>`

| Method | What it does |
|---|---|
| `GenerateToken(Guid userId)` | Creates a JWT access token with `sub` claim = userId, signed with `JWTSettings.Secret` using `HmacSha256`. Creates a refresh token (random base64 string). Returns `AuthTokens(accessToken, refreshToken, expiresAt)`. Access token expiry = `JWTSettings.ExpirationMinutes`. |
| `ValidateToken(string token)` | Validates the JWT signature and expiry using `TokenValidationParameters`. Extracts and returns the `sub` claim as `Guid`. Returns `null` if invalid/expired. |

**Key classes used:** `JwtSecurityTokenHandler`, `SigningCredentials`, `SymmetricSecurityKey`, `SecurityTokenDescriptor`, `TokenValidationParameters`

### 4b — `Authentication/HashService.cs`

**Implements:** `IHashService`
**Uses:** `BCrypt.Net-Next` NuGet package

| Method | What it does |
|---|---|
| `Hash(string input)` | Returns `BCrypt.Net.BCrypt.HashPassword(input)` — generates a salted bcrypt hash |

**Important note:** Your `LoginUserCommandHandler` currently compares hashes with `!=`. BCrypt hashes are salted, so you can't compare two hash strings directly. You'll eventually need a `Verify(string input, string hash)` method on this interface, or handle verification in the handler using `BCrypt.Net.BCrypt.Verify()`. This is a bug to fix later in the Application layer.

---

## Step 5: External Service Implementations

**Directory:** `Services/` (already exists)

### 5a — `Services/iTunesSearchService.cs`

**Implements:** `IPodcastSearchService`
**Injects:** `HttpClient` (via `IHttpClientFactory` / typed client)

**What it does:**
- Calls `https://itunes.apple.com/search?term={query}&entity=podcast&limit={pageSize}&offset={page*pageSize}`
- Deserializes the JSON response into an internal `iTunesResponse` model
- Maps each result to `PodcastSearchResult(Guid.NewGuid(), title, artistName, feedUrl, artworkUrl600)`
- Returns `List<PodcastSearchResult>`

**Internal models needed (private/nested classes):**
```
iTunesResponse { int ResultCount, List<iTunesResult> Results }
iTunesResult { string TrackName, string ArtistName, string FeedUrl, string ArtworkUrl600 }
```

**Error handling:** Wrap in try/catch, return empty list on failure. Check `response.IsSuccessStatusCode` before deserializing.

### 5b — `Services/RssFeedParser.cs`

**Implements:** `IRssParser`
**Injects:** `HttpClient` (via `IHttpClientFactory` / typed client)

**What it does:**
- Downloads the RSS XML from the feed URL using `HttpClient`
- Parses with `System.Xml.Linq.XDocument`
- Extracts podcast metadata from `<channel>`: title, author (`<itunes:author>`), description, artwork (`<itunes:image>`)
- Extracts episodes from `<item>` elements: title, description, media URL (`<enclosure url="...">`), duration (`<itunes:duration>`), published date (`<pubDate>`)
- Creates a `Podcast` entity and calls `podcast.AddEpisode()` for each parsed episode
- Returns the fully populated `Podcast`

**Key XML namespaces:**
```csharp
XNamespace itunes = "http://www.itunes.com/dtds/podcast-1.0.dtd";
```

**Duration parsing:** `<itunes:duration>` can be `HH:MM:SS`, `MM:SS`, or just seconds. Handle all three formats.

### 5c — `Services/CachedPodcastSearchService.cs` (optional, can defer)

**Implements:** `IPodcastSearchService` (decorator pattern)
**Injects:** `IPodcastSearchService` (the iTunes one), `IDistributedCache`

**What it does:**
- Before calling iTunes, checks Redis cache with key `podcast-search:{query}:{page}:{pageSize}`
- If cached, deserialize and return
- If not cached, delegate to the inner `iTunesSearchService`, cache the result for 1 hour, return
- This avoids hitting iTunes API rate limits on repeated searches

**Can be deferred:** Skip this initially. Register `iTunesSearchService` directly as `IPodcastSearchService`. Add caching later.

---

## Step 6: Dependency Injection Registration

**File:** `DependencyInjection.cs` (create at project root)

**What it does:**
- Defines `public static class DependencyInjection` with extension method `AddInfrastructure(this IServiceCollection services, IConfiguration configuration)`
- Registers everything the infrastructure layer provides:

```
// DbContext
services.AddDbContext<SyncPodcastDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("Default")));

// Repositories
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IPodcastRepository, PodcastRepository>();
services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
services.AddScoped<IPlaybackProgressRepository, PlaybackProgressRepository>();

// Authentication
services.Configure<JWTSettings>(configuration.GetSection("Jwt"));
services.AddScoped<ITokenService, TokenService>();
services.AddSingleton<IHashService, HashService>();

// External services
services.AddHttpClient<IPodcastSearchService, iTunesSearchService>(client =>
    client.BaseAddress = new Uri("https://itunes.apple.com/"));
services.AddHttpClient<IRssParser, RssFeedParser>();

// (Optional) Redis caching
// services.AddStackExchangeRedisCache(options =>
//     options.Configuration = configuration.GetConnectionString("Redis"));
// services.Decorate<IPodcastSearchService, CachedPodcastSearchService>();
```

**This is what the API's `Program.cs` will call:** `builder.Services.AddInfrastructure(builder.Configuration);`

---

## File Summary — What to Create

| # | File Path | Implements | Directory |
|---|---|---|---|
| 0 | `SyncPoscast.Infrastructure.csproj` | Add NuGet packages | (exists) |
| 1 | `Persistence/SyncPodcastDbContext.cs` | DbContext with 5 DbSets | Create `Persistence/` |
| 2a | `Configurations/UserConfiguration.cs` | `IEntityTypeConfiguration<User>` | exists |
| 2b | `Configurations/PodcastConfiguration.cs` | `IEntityTypeConfiguration<Podcast>` | exists |
| 2c | `Configurations/EpisodeConfiguration.cs` | `IEntityTypeConfiguration<Episode>` | exists |
| 2d | `Configurations/SubscriptionConfiguration.cs` | `IEntityTypeConfiguration<Subscription>` | exists |
| 2e | `Configurations/PlaybackProgressConfiguration.cs` | `IEntityTypeConfiguration<PlaybackProgress>` | exists |
| 3a | `Repositories/UserRepository.cs` | `IUserRepository` | Create `Repositories/` |
| 3b | `Repositories/PodcastRepository.cs` | `IPodcastRepository` | Create `Repositories/` |
| 3c | `Repositories/SubscriptionRepository.cs` | `ISubscriptionRepository` | Create `Repositories/` |
| 3d | `Repositories/PlaybackProgressRepository.cs` | `IPlaybackProgressRepository` | Create `Repositories/` |
| 4a | `Authentication/TokenService.cs` | `ITokenService` | exists |
| 4b | `Authentication/HashService.cs` | `IHashService` | exists |
| 5a | `Services/iTunesSearchService.cs` | `IPodcastSearchService` | exists |
| 5b | `Services/RssFeedParser.cs` | `IRssParser` | exists |
| 5c | `Services/CachedPodcastSearchService.cs` | `IPodcastSearchService` (decorator) | exists (optional/defer) |
| 6 | `DependencyInjection.cs` | Service registration extension | root |

**Total: 15 files to create** (14 if you defer caching), plus package additions to the csproj.

---

## Recommended Build Order

Work in this order — each step builds on the previous:

1. **Packages** (Step 0) — nothing compiles without these
2. **DbContext** (Step 1) — foundation for all data access
3. **Entity Configurations** (Step 2a-2e) — defines the database schema
4. **Repositories** (Step 3a-3d) — implements the domain interfaces the handlers depend on
5. **HashService** (Step 4b) — simplest service, one method, no dependencies
6. **TokenService** (Step 4a) — auth handlers need this
7. **RssFeedParser** (Step 5b) — subscribe handler needs this
8. **iTunesSearchService** (Step 5a) — search query handler needs this
9. **DependencyInjection** (Step 6) — wires everything together
10. **CachedPodcastSearchService** (Step 5c) — optional, add after everything works

After the Infrastructure layer is done, you'll move to the API layer to wire up `Program.cs` and create controllers.
