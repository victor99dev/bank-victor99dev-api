namespace bank.victor99dev.Application.Interfaces.Messaging;

public interface IEventSerializer
{
    string Serialize(object evt);
    string GetEventType(object evt);
}