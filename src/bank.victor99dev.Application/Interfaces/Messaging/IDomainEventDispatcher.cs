using bank.victor99dev.Domain.Events;

namespace bank.victor99dev.Application.Interfaces.Messaging;

public interface IDomainEventDispatcher
{
    Task EnqueueAsync(IEnumerable<IDomainEvent> events, string? correlationId, CancellationToken ct);
}