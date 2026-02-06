using System.Text.Json;
using bank.victor99dev.Application.Dtos;

namespace bank.victor99dev.Infrastructure.Messaging.Outbox.Envelope;

public static class OutboxEnvelopeBuilder
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static string Build(OutboxItem item)
    {
        using var doc = JsonDocument.Parse(item.Payload);

        var envelope = new OutboxEventEnvelope(
            Type: item.Type,
            EventId: item.Id,
            OccurredOnUtc: item.OccurredOnUtc,
            CorrelationId: item.CorrelationId,
            AggregateId: item.AggregateId,
            Key: item.Key,
            Payload: doc.RootElement.Clone()
        );

        return JsonSerializer.Serialize(envelope, Options);
    }
}
