using bank.victor99dev.Domain.Interfaces.Events;

namespace bank.victor99dev.Domain.Events;

public sealed record AccountUpdatedDomainEvent(Guid EventId, DateTimeOffset OccurredOnUtc, Guid AccountId) : IDomainEvent, IHasAggregateKey
{
    public string AggregateId => AccountId.ToString("N");
    public string? Key => AggregateId;
}