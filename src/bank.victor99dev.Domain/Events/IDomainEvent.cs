namespace bank.victor99dev.Domain.Events;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredOnUtc { get; }
}