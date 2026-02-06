namespace bank.victor99dev.Domain.Interfaces.Events;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredOnUtc { get; }
}