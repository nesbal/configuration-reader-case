using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConfigurationReader.Models;

public class ConfigurationItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string ApplicationName { get; set; } = string.Empty;
}