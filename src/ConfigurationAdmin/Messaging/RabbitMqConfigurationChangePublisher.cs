using System.Text;
using System.Text.Json;
using ConfigurationReader.Models;
using RabbitMQ.Client;

namespace ConfigurationAdmin.Messaging;

public class RabbitMqConfigurationChangePublisher : IConfigurationChangePublisher, IDisposable
{
    private const string ExchangeName = "configuration-changes";

    private readonly IConnection? _connection;
    private readonly IModel? _channel;

    public RabbitMqConfigurationChangePublisher(IConfiguration configuration)
    {
        try
        {
            var connectionString = configuration["RabbitMQ:ConnectionString"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return;
            }

            var factory = new ConnectionFactory
            {
                Uri = new Uri(connectionString)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                exchange: ExchangeName,
                type: ExchangeType.Fanout,
                durable: true);
        }
        catch
        {
            // RabbitMQ erişilemezse admin kayıt işlemleri çalışmaya devam eder.
        }
    }

    public void Publish(string applicationName, string key)
    {
        if (_channel is null)
        {
            return;
        }

        var message = new ConfigurationChangedMessage
        {
            ApplicationName = applicationName,
            Key = key
        };

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(
            exchange: ExchangeName,
            routingKey: string.Empty,
            basicProperties: null,
            body: body);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}