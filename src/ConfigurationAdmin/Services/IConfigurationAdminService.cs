using ConfigurationReader.Models;

namespace ConfigurationAdmin.Services;

public interface IConfigurationAdminService
{
    Task<IReadOnlyList<ConfigurationItem>> GetAllAsync();
    Task<ConfigurationItem?> GetByIdAsync(string id);
    Task CreateAsync(ConfigurationItem item);
    Task UpdateAsync(ConfigurationItem item);
}