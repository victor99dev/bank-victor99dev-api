using bank.victor99dev.Domain.Interfaces.Events;

namespace bank.victor99dev.Domain.Events;

public sealed record AccountNameChangedDomainEvent(Guid EventId, DateTimeOffset OccurredOnUtc, Guid AccountId, string OldName, string NewName) : IDomainEvent, IHasAggregateKey
{
    public string AggregateId => AccountId.ToString("N");
    public string? Key => AggregateId;
}