using bank.victor99dev.Application.Interfaces.Caching;
using bank.victor99dev.Application.Interfaces.Messaging;
using bank.victor99dev.Application.Interfaces.Repository;
using bank.victor99dev.Application.Shared.Messaging;
using bank.victor99dev.Infrastructure.Caching.Redis;
using bank.victor99dev.Infrastructure.Configurations;
using bank.victor99dev.Infrastructure.Database.Context;
using bank.victor99dev.Infrastructure.Database.Repositories;
using bank.victor99dev.Infrastructure.Database.UnitOfWork;
using bank.victor99dev.Infrastructure.Messaging.Kafka;
using bank.victor99dev.Infrastructure.Messaging.Outbox.Persistence;
using bank.victor99dev.Infrastructure.Messaging.Outbox.Processing;
using bank.victor99dev.Infrastructure.Messaging.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace bank.victor99dev.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDatabase(configuration)
            .AddCaching(configuration)
            .AddRepositories()
            .AddMessaging(configuration);

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
            options.InstanceName = "bank-victor99dev:";
        });

        services.AddScoped<IAccountCacheRepository, AccountCacheRepository>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAccountRepository, AccountRepository>();

        return services;
    }

    private static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaConfigurationOptions>(configuration.GetSection("Kafka"));

        services.AddSingleton<IEventSerializer, JsonEventSerializer>();
        services.AddSingleton<IEventBusPublisher, KafkaPublisher>();

        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddHostedService<OutboxProcessorWorker>();

        services.AddScoped<IAccountEventFactory, AccountEventFactory>();

        return services;
    }
}
