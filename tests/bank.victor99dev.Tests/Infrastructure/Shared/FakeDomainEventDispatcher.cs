using bank.victor99dev.Application.Interfaces.Messaging;
using bank.victor99dev.Domain.Interfaces.Events;

namespace bank.victor99dev.Tests.Infrastructure.Shared;

public sealed class FakeDomainEventDispatcher : IDomainEventDispatcher
{
    public List<IDomainEvent> Enqueued { get; } = [];
    public int EnqueueCalls { get; private set; }

    public Task EnqueueAsync(IEnumerable<IDomainEvent> events, string? correlationId = null, CancellationToken cancellationToken = default)
    {
        EnqueueCalls++;
        Enqueued.AddRange(events);
        return Task.CompletedTask;
    }
}
