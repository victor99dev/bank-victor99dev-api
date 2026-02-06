using bank.victor99dev.Domain.Events.Bodies;
using bank.victor99dev.Domain.Interfaces.Events;

namespace bank.victor99dev.Domain.Events;

public sealed record class AccountCpfChangedDomainEvent : IDomainEvent, IHasAggregateKey
{
    public Guid EventId { get; init; }
    public DateTimeOffset OccurredOnUtc { get; init; }
    public AccountCpfChangedBody Body { get; init; } = new();

    public string AggregateId => Body.AccountId.ToString("N");
    public string? Key => AggregateId;
}