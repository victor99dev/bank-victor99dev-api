using System.Text.Json;
using bank.victor99dev.Application.Interfaces.Messaging;

namespace bank.victor99dev.Infrastructure.Messaging;

public sealed class JsonEventSerializer : IEventSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public string Serialize(object evt) =>
        JsonSerializer.Serialize(evt, evt.GetType(), Options);

    public string GetEventType(object evt) =>
        evt.GetType().FullName ?? evt.GetType().Name;
}
