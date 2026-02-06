namespace bank.victor99dev.Domain.Interfaces.Events;

public interface IHasAggregateKey
{
    string AggregateId { get; }
    string? Key { get; }
}