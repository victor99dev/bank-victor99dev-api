namespace bank.victor99dev.Application.Dtos;

public sealed record OutboxItem
{
    public Guid Id { get; init; }
    public DateTimeOffset OccurredOnUtc { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Payload { get; init; } = string.Empty;
    public string? CorrelationId { get; init; }
    public string? AggregateId { get; init; }
    public string? Key { get; init; }
}