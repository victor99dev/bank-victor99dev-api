using bank.victor99dev.Domain.Interfaces.Events;

namespace bank.victor99dev.Domain.Events;

public sealed record AccountCpfChangedDomainEvent(Guid EventId, DateTimeOffset OccurredOnUtc, Guid AccountId, string OldCpf, string NewCpf) : IDomainEvent, IHasAggregateKey
{
    public string AggregateId => AccountId.ToString("N");
    public string? Key => AggregateId;
}