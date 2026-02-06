using bank.victor99dev.Application.Interfaces.Messaging;
using bank.victor99dev.Infrastructure.Configurations;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace bank.victor99dev.Infrastructure.Messaging.Kafka
{
    public class KafkaPublisher : IEventBusPublisher, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly KafkaConfigurationOptions _kafkaConfigurationOptions;

        public KafkaPublisher(IOptions<KafkaConfigurationOptions> kafkaConfigurationOptions)
        {
            var opt = kafkaConfigurationOptions.Value;

            var config = new ProducerConfig
            {
                BootstrapServers = opt.BootstrapServers,
                ClientId = opt.ClientId,
                Acks = Acks.All,
                EnableIdempotence = true
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }
        public async Task PublishAsync(string topic, string key, string message, CancellationToken cancellationToken)
        {
            var msg = new Message<string, string> { Key = key, Value = message };

            await _producer.ProduceAsync(topic, msg, cancellationToken);
        }

        public void Dispose() => _producer.Dispose();
    }
}