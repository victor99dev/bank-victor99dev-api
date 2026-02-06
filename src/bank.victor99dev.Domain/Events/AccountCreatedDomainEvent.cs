namespace bank.victor99dev.Domain.Events;

public sealed record AccountCreatedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOnUtc,
    Guid AccountId,
    string Cpf
) : IDomainEvent;