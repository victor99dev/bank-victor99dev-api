using bank.victor99dev.Application.Dtos;
using bank.victor99dev.Application.Interfaces.Messaging;
using bank.victor99dev.Application.Interfaces.Messaging.MessagingRepository;
using bank.victor99dev.Domain.Interfaces.Events;

namespace bank.victor99dev.Application.Shared.Messaging;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly IEventSerializer _eventSerializer;

    public DomainEventDispatcher(IOutboxRepository outboxRepository, IEventSerializer eventSerializer)
    {
        _outboxRepository = outboxRepository;
        _eventSerializer = eventSerializer;
    }

    public Task EnqueueAsync(IEnumerable<IDomainEvent> events, string? correlationId, CancellationToken cancellationToken)
    {
        var items = events.Select(e =>
        {
            var type = e.GetType().FullName ?? e.GetType().Name;
            var payload = _eventSerializer.Serialize(e);

            var agg = e as IHasAggregateKey;

            return new OutboxItem
            {
                Id = e.EventId,
                OccurredOnUtc = e.OccurredOnUtc,
                Type = type,
                Payload = payload,
                CorrelationId = correlationId,
                AggregateId = agg?.AggregateId,
                Key = agg?.Key
            };
        });

        return _outboxRepository.AddRangeAsync(items, cancellationToken);
    }
}
