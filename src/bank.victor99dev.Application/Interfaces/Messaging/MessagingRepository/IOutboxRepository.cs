using bank.victor99dev.Application.Dtos;

namespace bank.victor99dev.Application.Interfaces.Messaging.MessagingRepository;

public interface IOutboxRepository
{
    Task AddRangeAsync(IEnumerable<OutboxItem> outboxItems, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OutboxItem>> ClaimBatchAsync(
        int take,
        string workerId,
        TimeSpan lease,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken);

    Task MarkProcessedAsync(Guid id, DateTimeOffset processedOnUtc, CancellationToken cancellationToken = default);

    Task MarkFailedAsync(
        Guid id,
        string error,
        DateTimeOffset nowUtc,
        int maxAttempts,
        TimeSpan baseBackoff,
        CancellationToken cancellationToken);
}
