namespace bank.victor99dev.Infrastructure.Configurations;

public class KafkaConfigurationOptions
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string TopicOutbox { get; set; } = string.Empty;
}