using bank.victor99dev.Domain.Events.Bodies;
using bank.victor99dev.Domain.Primitives;

namespace bank.victor99dev.Domain.Events;

public sealed record class AccountUpdatedDomainEvent : IDomainEvent, IHasAggregateKey
{
    public Guid EventId { get; init; }
    public DateTimeOffset OccurredOnUtc { get; init; }
    public AccountUpdatedBody Body { get; init; } = new();
    public string AggregateId => Body.AccountId.ToString("N");
    public string? Key => AggregateId;
}