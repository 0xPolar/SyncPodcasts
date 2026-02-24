using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SyncPodcast.Domain.Interfaces;
using SyncPodcast.Infrastructure.Authentication;
using SyncPodcast.Infrastructure.Persistence;
using SyncPodcast.Infrastructure.Repositories;
using SyncPodcast.Infrastructure.Services;

namespace SyncPodcast.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // EF Core with PostgreSQL
        services.AddDbContext<SyncPodcastDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        // JWT settings — bind from "Jwt" section in appsettings.json
        var jwtSettings = configuration.GetSection("Jwt").Get<JWTSettings>()!;
        services.AddSingleton(jwtSettings);

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPodcastRepository, PodcastRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IPlaybackProgressRepository, PlaybackProgressRepository>();

        // Services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IHashService, HashService>();
        services.AddScoped<IRssParser, RssParser>();
        services.AddScoped<IPodcastSearchService, PodcastSearchService>();

        // HttpClient for RSS parsing
        services.AddHttpClient();

        return services;
    }
}
