using ConfigurationReader.Models;
using MongoDB.Driver;
using ConfigurationAdmin.Messaging;

namespace ConfigurationAdmin.Services;

public class ConfigurationAdminService : IConfigurationAdminService
{
    private readonly IMongoCollection<ConfigurationItem> _collection;

    private readonly IConfigurationChangePublisher _publisher;

    public ConfigurationAdminService(
        IConfiguration configuration,
        IConfigurationChangePublisher publisher)
    {
        _publisher = publisher;

        var connectionString = configuration["MongoDb:ConnectionString"];
        var databaseName = configuration["MongoDb:DatabaseName"];
        var collectionName = configuration["MongoDb:CollectionName"];

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        _collection = database.GetCollection<ConfigurationItem>(collectionName);
    }

    public async Task<IReadOnlyList<ConfigurationItem>> GetAllAsync()
    {
        return await _collection
            .Find(_ => true)
            .SortBy(x => x.ApplicationName)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<ConfigurationItem?> GetByIdAsync(string id)
    {
        return await _collection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(ConfigurationItem item)
    {
        await _collection.InsertOneAsync(item);

        _publisher.Publish(item.ApplicationName, item.Name);
    }

    public async Task UpdateAsync(ConfigurationItem item)
    {
        await _collection.ReplaceOneAsync(x => x.Id == item.Id, item);

        _publisher.Publish(item.ApplicationName, item.Name);
    }
}