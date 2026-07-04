using ConfigurationReader.Abstractions;
using ConfigurationReader.Converters;
using ConfigurationReader.Models;
using ConfigurationReader.Storage;
using ConfigurationReader.Messaging;

namespace ConfigurationReader;

public class ConfigurationReader : IDisposable
{
    private readonly string _applicationName;
    private readonly IConfigurationStorage _storage;
    private readonly Timer _timer;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private readonly RabbitMqConfigurationChangeSubscriber? _subscriber;

    private IReadOnlyDictionary<string, ConfigurationItem> _cache =
        new Dictionary<string, ConfigurationItem>();

    public ConfigurationReader(
        string applicationName,
        string connectionString,
        int refreshTimerIntervalInMs)
    {
        _applicationName = applicationName;
        _storage = new MongoConfigurationStorage(connectionString);

        RefreshAsync().GetAwaiter().GetResult();

        _timer = new Timer(
            async _ => await RefreshAsync(),
            null,
            refreshTimerIntervalInMs,
            refreshTimerIntervalInMs);
        var rabbitMqConnectionString = Environment.GetEnvironmentVariable("RabbitMQ__ConnectionString");

        if (!string.IsNullOrWhiteSpace(rabbitMqConnectionString))
        {
            _subscriber = new RabbitMqConfigurationChangeSubscriber(
                rabbitMqConnectionString,
                _applicationName,
                RefreshAsync);
        }
    }

    internal ConfigurationReader(
        string applicationName,
        IConfigurationStorage storage)
    {
        _applicationName = applicationName;
        _storage = storage;
        _timer = new Timer(_ => { });

        RefreshAsync().GetAwaiter().GetResult();
    }

    public T GetValue<T>(string key)
    {
        if (!_cache.TryGetValue(key, out var item))
        {
            throw new KeyNotFoundException($"Configuration key not found: {key}");
        }

        return ConfigurationValueConverter.ConvertValue<T>(item.Value, item.Type);
    }

    internal async Task RefreshAsync()
    {
        if (!await _refreshLock.WaitAsync(0))
        {
            return;
        }

        try
        {
            var items = await _storage.GetActiveConfigurationsAsync(_applicationName);

            _cache = items.ToDictionary(
                x => x.Name,
                x => x,
                StringComparer.OrdinalIgnoreCase);
        }
        catch
        {
            // Storage'a erişilemezse son başarılı cache ile devam edilir.
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
        _subscriber?.Dispose();
        _refreshLock.Dispose();
    }
}