namespace ConfigurationAdmin.Messaging;

public interface IConfigurationChangePublisher
{
    void Publish(string applicationName, string key);
}