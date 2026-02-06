using bank.victor99dev.Application.Dtos;
using bank.victor99dev.Application.Interfaces.Messaging.MessagingRepository;
using bank.victor99dev.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace bank.victor99dev.Infrastructure.Messaging.Outbox;

public sealed class OutboxRepository : IOutboxRepository
{
    private readonly AppDbContext _db;

    public OutboxRepository(AppDbContext db) => _db = db;

    public Task AddRangeAsync(IEnumerable<OutboxItem> items, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        _db.OutboxMessages.AddRange(items.Select(i => new OutboxMessage
        {
            Id = i.Id,
            OccurredOnUtc = i.OccurredOnUtc,
            Type = i.Type,
            Payload = i.Payload,
            CorrelationId = i.CorrelationId,
            AggregateId = i.AggregateId,
            Key = i.Key,
            Attempts = 0,
            CreatedOnUtc = now
        }));

        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<OutboxItem>> ClaimBatchAsync(
        int take,
        string workerId,
        TimeSpan lease,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken)
    {
        var leaseUntil = nowUtc.Add(lease);

        var candidateIds = await _db.OutboxMessages
            .AsNoTracking()
            .Where(x => x.ProcessedOnUtc == null)
            .Where(x => x.NextAttemptOnUtc == null || x.NextAttemptOnUtc <= nowUtc)
            .Where(x => x.LockedUntilUtc == null || x.LockedUntilUtc < nowUtc)
            .OrderBy(x => x.OccurredOnUtc)
            .Select(x => x.Id)
            .Take(take * 3)
            .ToListAsync(cancellationToken);

        if (candidateIds.Count == 0)
            return Array.Empty<OutboxItem>();

        var lockedIds = new List<Guid>(capacity: take);

        foreach (var id in candidateIds)
        {
            if (lockedIds.Count >= take)
                break;

            var affected = await _db.OutboxMessages
                .Where(x => x.Id == id)
                .Where(x => x.ProcessedOnUtc == null)
                .Where(x => x.NextAttemptOnUtc == null || x.NextAttemptOnUtc <= nowUtc)
                .Where(x => x.LockedUntilUtc == null || x.LockedUntilUtc < nowUtc)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.LockedBy, workerId)
                    .SetProperty(x => x.LockedUntilUtc, leaseUntil),
                    cancellationToken);

            if (affected == 1)
                lockedIds.Add(id);
        }

        if (lockedIds.Count == 0)
            return Array.Empty<OutboxItem>();

        var rows = await _db.OutboxMessages
            .AsNoTracking()
            .Where(x => lockedIds.Contains(x.Id))
            .OrderBy(x => x.OccurredOnUtc)
            .Select(x => new OutboxItem
            {
                Id = x.Id,
                OccurredOnUtc = x.OccurredOnUtc,
                Type = x.Type,
                Payload = x.Payload,
                CorrelationId = x.CorrelationId,
                AggregateId = x.AggregateId,
                Key = x.Key
            })
            .ToListAsync(cancellationToken);

        return rows;
    }

    public Task MarkProcessedAsync(Guid id, DateTimeOffset processedOnUtc, CancellationToken cancellationToken) =>
        _db.OutboxMessages
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.ProcessedOnUtc, processedOnUtc)
                .SetProperty(x => x.Error, (string?)null)
                .SetProperty(x => x.LockedBy, (string?)null)
                .SetProperty(x => x.LockedUntilUtc, (DateTimeOffset?)null),
                cancellationToken);

    public async Task MarkFailedAsync(
        Guid id,
        string error,
        DateTimeOffset nowUtc,
        int maxAttempts,
        TimeSpan baseBackoff,
        CancellationToken cancellationToken)
    {
        var current = await _db.OutboxMessages
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => x.Attempts)
            .SingleAsync(cancellationToken);

        var attempts = current + 1;

        DateTimeOffset? nextAttempt = null;

        if (attempts < maxAttempts)
        {
            var seconds = Math.Min(baseBackoff.TotalSeconds * Math.Pow(2, attempts - 1), 300);
            nextAttempt = nowUtc.AddSeconds(seconds);
        }

        await _db.OutboxMessages
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.Attempts, attempts)
                .SetProperty(x => x.Error, error)
                .SetProperty(x => x.NextAttemptOnUtc, nextAttempt)
                .SetProperty(x => x.LockedBy, (string?)null)
                .SetProperty(x => x.LockedUntilUtc, (DateTimeOffset?)null),
                cancellationToken);
    }
}
