using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR scans this assembly for all IRequestHandler<,> implementations
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // FluentValidation scans this assembly for all AbstractValidator<T> implementations
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Pipeline behaviors run in order for every MediatR request:
        // ValidationBehavior validates first, then LoggingBehavior, then PerformanceBehavior
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

        return services;
    }
}
