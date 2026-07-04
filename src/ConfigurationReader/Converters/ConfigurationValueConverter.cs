namespace ConfigurationReader.Converters;

public static class ConfigurationValueConverter
{
    public static T ConvertValue<T>(string value, string type)
    {
        object convertedValue = type.ToLowerInvariant() switch
        {
            "string" => value,
            "int" or "integer" => int.Parse(value),
            "double" => double.Parse(value),
            "bool" or "boolean" => value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase),
            _ => throw new NotSupportedException($"Unsupported configuration type: {type}")
        };

        return (T)convertedValue;
    }
}