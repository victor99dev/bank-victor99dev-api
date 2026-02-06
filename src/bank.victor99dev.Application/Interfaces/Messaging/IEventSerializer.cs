namespace bank.victor99dev.Application.Interfaces.Messaging;

public interface IEventSerializer
{
    string Serialize<T>(T evt);
    string GetEventType<T>();
}