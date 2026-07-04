using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConfigurationReader.Models;

public class ConfigurationItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [RegularExpression("string|int|integer|double|bool|boolean")]
    public string Type { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    [Required]
    public string ApplicationName { get; set; } = string.Empty;
}