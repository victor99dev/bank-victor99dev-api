using bank.victor99dev.Domain.Interfaces.Events;

namespace bank.victor99dev.Application.Interfaces.Messaging;

public interface IDomainEventDispatcher
{
    Task EnqueueAsync(IEnumerable<IDomainEvent> events, string? correlationId = null, CancellationToken cancellationToken = default);
}