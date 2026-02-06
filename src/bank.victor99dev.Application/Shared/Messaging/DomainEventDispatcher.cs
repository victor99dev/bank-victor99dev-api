using bank.victor99dev.Application.Dtos;
using bank.victor99dev.Application.Interfaces.Messaging;
using bank.victor99dev.Application.Interfaces.Messaging.MessagingRepository;
using bank.victor99dev.Domain.Events;

namespace bank.victor99dev.Application.Shared.Messaging;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly IEventSerializer _eventSerializer;

    public DomainEventDispatcher(IOutboxRepository outboxRepository, IEventSerializer eventSerializer)
    {
        _outboxRepository = outboxRepository;
        _eventSerializer = eventSerializer;
    }
    public Task EnqueueAsync(IEnumerable<IDomainEvent> events, string? correlationId, CancellationToken ct)
    {
        var items = events.Select(e =>
        {
            var type = e.GetType().FullName ?? e.GetType().Name;
            var payload = _eventSerializer.Serialize(e);

            var aggregateId = e switch
            {
                AccountCreatedDomainEvent a => a.AccountId.ToString("N"),
                _ => null
            };

            return new OutboxItem
            {
                Id = e.EventId,
                OccurredOnUtc = e.OccurredOnUtc,
                Type = type,
                Payload = payload,
                CorrelationId = correlationId,
                AggregateId = aggregateId,
                Key = aggregateId
            };
        });

        return _outboxRepository.AddRangeAsync(items, ct);
    }
}