namespace ConfigurationReader.Models;

public class ConfigurationChangedMessage
{
    public string ApplicationName { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}