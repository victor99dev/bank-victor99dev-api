using System.Text.Json;
using bank.victor99dev.Application.Interfaces.Messaging;

namespace bank.victor99dev.Infrastructure.Messaging;

public class JsonEventSerializer : IEventSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public string Serialize<T>(T evt) => JsonSerializer.Serialize(evt, Options);

    public string GetEventType<T>() => typeof(T).FullName ?? typeof(T).Name;
}