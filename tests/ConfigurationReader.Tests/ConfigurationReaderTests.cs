using ConfigurationReader.Models;
using Reader = ConfigurationReader.ConfigurationReader;

namespace ConfigurationReader.Tests;

public class ConfigurationReaderTests
{
    [Fact]
    public void GetValue_ReturnsStringValue()
    {
        var reader = CreateReader();

        var result = reader.GetValue<string>("SiteName");

        Assert.Equal("soty.io", result);
    }

    [Fact]
    public void GetValue_ReturnsBoolValue()
    {
        var reader = CreateReader();

        var result = reader.GetValue<bool>("IsBasketEnabled");

        Assert.True(result);
    }

    [Fact]
    public void GetValue_ReturnsIntValue()
    {
        var reader = CreateReader();

        var result = reader.GetValue<int>("MaxItemCount");

        Assert.Equal(50, result);
    }

    [Fact]
    public void GetValue_ThrowsException_WhenKeyDoesNotExist()
    {
        var reader = CreateReader();

        Assert.Throws<KeyNotFoundException>(() =>
            reader.GetValue<string>("UnknownKey"));
    }

    private static Reader CreateReader()
    {
        var items = new List<ConfigurationItem>
        {
            new()
            {
                Id = "1",
                Name = "SiteName",
                Type = "string",
                Value = "soty.io",
                IsActive = true,
                ApplicationName = "SERVICE-A"
            },
            new()
            {
                Id = "2",
                Name = "IsBasketEnabled",
                Type = "bool",
                Value = "1",
                IsActive = true,
                ApplicationName = "SERVICE-A"
            },
            new()
            {
                Id = "3",
                Name = "MaxItemCount",
                Type = "int",
                Value = "50",
                IsActive = true,
                ApplicationName = "SERVICE-A"
            }
        };

        var storage = new FakeConfigurationStorage(items);

        return new Reader("SERVICE-A", storage);
    }
    
    [Fact]
    public void GetValue_DoesNotReturnInactiveConfiguration()
    {
        var items = new List<ConfigurationItem>
        {
            new()
            {
                Id = "1",
                Name = "MaxItemCount",
                Type = "int",
                Value = "50",
                IsActive = false,
                ApplicationName = "SERVICE-A"
            }
        };

        var storage = new FakeConfigurationStorage(items);
        var reader = new Reader("SERVICE-A", storage);

        Assert.Throws<KeyNotFoundException>(() =>
            reader.GetValue<int>("MaxItemCount"));
    }

    [Fact]
    public void GetValue_DoesNotReturnOtherApplicationConfiguration()
    {
        var items = new List<ConfigurationItem>
        {
            new()
            {
                Id = "1",
                Name = "IsBasketEnabled",
                Type = "bool",
                Value = "1",
                IsActive = true,
                ApplicationName = "SERVICE-B"
            }
        };

        var storage = new FakeConfigurationStorage(items);
        var reader = new Reader("SERVICE-A", storage);

        Assert.Throws<KeyNotFoundException>(() =>
            reader.GetValue<bool>("IsBasketEnabled"));
    }

    [Fact]
    public async Task RefreshAsync_UpdatesConfigurationValue()
    {
        var storage = new FakeConfigurationStorage(new List<ConfigurationItem>
        {
            new()
            {
                Id = "1",
                Name = "SiteName",
                Type = "string",
                Value = "old-value",
                IsActive = true,
                ApplicationName = "SERVICE-A"
            }
        });

        var reader = new Reader("SERVICE-A", storage);

        storage.SetItems(new List<ConfigurationItem>
        {
            new()
            {
                Id = "1",
                Name = "SiteName",
                Type = "string",
                Value = "new-value",
                IsActive = true,
                ApplicationName = "SERVICE-A"
            }
        });

        await reader.RefreshAsync();

        var result = reader.GetValue<string>("SiteName");

        Assert.Equal("new-value", result);
    }

    [Fact]
    public async Task RefreshAsync_KeepsLastSuccessfulCache_WhenStorageFails()
    {
        var storage = new FakeConfigurationStorage(new List<ConfigurationItem>
        {
            new()
            {
                Id = "1",
                Name = "SiteName",
                Type = "string",
                Value = "soty.io",
                IsActive = true,
                ApplicationName = "SERVICE-A"
            }
        });

        var reader = new Reader("SERVICE-A", storage);

        storage.ThrowException = true;

        await reader.RefreshAsync();

        var result = reader.GetValue<string>("SiteName");

        Assert.Equal("soty.io", result);
    }
}