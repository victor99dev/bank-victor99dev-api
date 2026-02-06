using System.Text.Json;

namespace bank.victor99dev.Infrastructure.Messaging.Outbox.Envelope;

public sealed record OutboxEventEnvelope(
    string Type,
    Guid EventId,
    DateTimeOffset OccurredOnUtc,
    string? CorrelationId,
    string? AggregateId,
    string? Key,
    JsonElement Payload
);