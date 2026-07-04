using ConfigurationReader.Models;

namespace ConfigurationReader.Abstractions;

public interface IConfigurationStorage
{
    Task<IReadOnlyList<ConfigurationItem>> GetActiveConfigurationsAsync(
        string applicationName,
        CancellationToken cancellationToken = default);
}