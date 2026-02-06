namespace bank.victor99dev.Application.Interfaces.Messaging;

public interface IEventBusPublisher
{
    Task PublishAsync(string topic, string key, string message, CancellationToken cancellationToken);
}