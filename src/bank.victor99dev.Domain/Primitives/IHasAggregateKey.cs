namespace bank.victor99dev.Domain.Primitives;

public interface IHasAggregateKey
{
    string AggregateId { get; }
    string? Key { get; }
}