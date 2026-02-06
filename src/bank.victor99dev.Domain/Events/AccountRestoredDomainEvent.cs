using bank.victor99dev.Domain.Primitives;

namespace bank.victor99dev.Domain.Events;

public sealed record class AccountRestoredDomainEvent : IDomainEvent, IHasAggregateKey
{
    public Guid EventId { get; init; }
    public DateTimeOffset OccurredOnUtc { get; init; }
    public Guid AccountId { get; init; }
    public string AggregateId => AccountId.ToString("N");
    public string? Key => AggregateId;
}