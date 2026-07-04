using ConfigurationReader.Abstractions;
using ConfigurationReader.Models;

namespace ConfigurationReader.Tests;

public class FakeConfigurationStorage : IConfigurationStorage
{
    private IReadOnlyList<ConfigurationItem> _items;

    public bool ThrowException { get; set; }

    public FakeConfigurationStorage(
        IReadOnlyList<ConfigurationItem> items,
        bool throwException = false)
    {
        _items = items;
        ThrowException = throwException;
    }

    public void SetItems(IReadOnlyList<ConfigurationItem> items)
    {
        _items = items;
    }

    public Task<IReadOnlyList<ConfigurationItem>> GetActiveConfigurationsAsync(
        string applicationName,
        CancellationToken cancellationToken = default)
    {
        if (ThrowException)
        {
            throw new Exception("Storage error");
        }

        var result = _items
            .Where(x => x.ApplicationName == applicationName && x.IsActive)
            .ToList();

        return Task.FromResult<IReadOnlyList<ConfigurationItem>>(result);
    }
}