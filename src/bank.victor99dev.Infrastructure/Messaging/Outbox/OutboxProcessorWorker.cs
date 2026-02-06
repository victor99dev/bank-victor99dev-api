using bank.victor99dev.Application.Interfaces.Messaging;
using bank.victor99dev.Application.Interfaces.Messaging.MessagingRepository;
using bank.victor99dev.Infrastructure.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace bank.victor99dev.Infrastructure.Messaging.Outbox;

public sealed class OutboxProcessorWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptionsMonitor<KafkaConfigurationOptions> _opt;
    private readonly ILogger<OutboxProcessorWorker> _logger;
    private readonly string _workerId = $"{Environment.MachineName}:{Guid.NewGuid():N}";

    public OutboxProcessorWorker(
        IServiceScopeFactory scopeFactory,
        IOptionsMonitor<KafkaConfigurationOptions> opt,
        ILogger<OutboxProcessorWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _opt = opt;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var cfg = _opt.CurrentValue;

            using var scope = _scopeFactory.CreateScope();
            var outbox = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
            var publisher = scope.ServiceProvider.GetRequiredService<IEventBusPublisher>();

            var now = DateTimeOffset.UtcNow;

            var batch = await outbox.ClaimBatchAsync(
                take: 50,
                workerId: _workerId,
                lease: TimeSpan.FromSeconds(30),
                nowUtc: now,
                cancellationToken: cancellationToken);

            if (batch.Count == 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                continue;
            }

            foreach (var item in batch)
            {
                try
                {
                    var key = item.Key ?? item.AggregateId ?? item.CorrelationId ?? item.Id.ToString("N");

                    await publisher.PublishAsync(cfg.TopicOutbox, key, item.Payload, cancellationToken);

                    await outbox.MarkProcessedAsync(item.Id, DateTimeOffset.UtcNow, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao publicar Outbox {OutboxId}", item.Id);

                    await outbox.MarkFailedAsync(
                        id: item.Id,
                        error: ex.Message,
                        nowUtc: DateTimeOffset.UtcNow,
                        maxAttempts: 10,
                        baseBackoff: TimeSpan.FromSeconds(2),
                        cancellationToken: cancellationToken);
                }
            }

            await Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken);
        }
    }
}
