using System.Text;
using System.Text.Json;
using ConfigurationReader.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ConfigurationReader.Messaging;

public class RabbitMqConfigurationChangeSubscriber : IDisposable
{
    private const string ExchangeName = "configuration-changes";

    private readonly IConnection? _connection;
    private readonly IModel? _channel;

    public RabbitMqConfigurationChangeSubscriber(
        string rabbitMqConnectionString,
        string applicationName,
        Func<Task> refreshAsync)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(rabbitMqConnectionString),
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                exchange: ExchangeName,
                type: ExchangeType.Fanout,
                durable: true);

            var queueName = _channel.QueueDeclare(
                queue: string.Empty,
                durable: false,
                exclusive: true,
                autoDelete: true).QueueName;

            _channel.QueueBind(
                queue: queueName,
                exchange: ExchangeName,
                routingKey: string.Empty);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (_, eventArgs) =>
            {
                var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var message = JsonSerializer.Deserialize<ConfigurationChangedMessage>(json);

                if (message?.ApplicationName == applicationName)
                {
                    await refreshAsync();
                }
            };

            _channel.BasicConsume(
                queue: queueName,
                autoAck: true,
                consumer: consumer);
        }
        catch
        {
            // RabbitMQ erişilemezse reader timer-based refresh ile çalışmaya devam eder.
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}