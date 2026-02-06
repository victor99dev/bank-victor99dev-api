namespace bank.victor99dev.Infrastructure.Messaging.Outbox.Persistence;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public DateTimeOffset OccurredOnUtc { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public string? CorrelationId { get; set; }
    public string? AggregateId { get; set; }
    public string? Key { get; set; }
    public DateTimeOffset? ProcessedOnUtc { get; set; }
    public string? Error { get; set; }
    public int Attempts { get; set; }
    public DateTimeOffset? NextAttemptOnUtc { get; set; }
    public string? LockedBy { get; set; }
    public DateTimeOffset? LockedUntilUtc { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
}
