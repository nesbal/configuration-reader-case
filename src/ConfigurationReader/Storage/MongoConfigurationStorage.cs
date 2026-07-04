using ConfigurationReader.Abstractions;
using ConfigurationReader.Models;
using MongoDB.Driver;

namespace ConfigurationReader.Storage;

public class MongoConfigurationStorage : IConfigurationStorage
{
    private const string DefaultDatabaseName = "ConfigurationDb";
    private const string CollectionName = "Configurations";

    private readonly IMongoCollection<ConfigurationItem> _collection;

    public MongoConfigurationStorage(string connectionString)
    {
        var mongoUrl = new MongoUrl(connectionString);
        var client = new MongoClient(connectionString);

        var databaseName = string.IsNullOrWhiteSpace(mongoUrl.DatabaseName)
            ? DefaultDatabaseName
            : mongoUrl.DatabaseName;

        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<ConfigurationItem>(CollectionName);
    }

    public async Task<IReadOnlyList<ConfigurationItem>> GetActiveConfigurationsAsync(
        string applicationName,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<ConfigurationItem>.Filter.And(
            Builders<ConfigurationItem>.Filter.Eq(x => x.ApplicationName, applicationName),
            Builders<ConfigurationItem>.Filter.Eq(x => x.IsActive, true)
        );

        return await _collection
            .Find(filter)
            .ToListAsync(cancellationToken);
    }
}