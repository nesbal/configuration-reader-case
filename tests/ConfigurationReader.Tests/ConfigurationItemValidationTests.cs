using System.ComponentModel.DataAnnotations;
using ConfigurationReader.Models;

namespace ConfigurationReader.Tests;

public class ConfigurationItemValidationTests
{
    [Fact]
    public void ConfigurationItem_IsInvalid_WhenNameIsEmpty()
    {
        var item = CreateValidItem();
        item.Name = string.Empty;

        var results = Validate(item);

        Assert.Contains(results, x => x.MemberNames.Contains(nameof(ConfigurationItem.Name)));
    }

    [Fact]
    public void ConfigurationItem_IsInvalid_WhenApplicationNameIsEmpty()
    {
        var item = CreateValidItem();
        item.ApplicationName = string.Empty;

        var results = Validate(item);

        Assert.Contains(results, x => x.MemberNames.Contains(nameof(ConfigurationItem.ApplicationName)));
    }

    [Fact]
    public void ConfigurationItem_IsInvalid_WhenTypeIsUnsupported()
    {
        var item = CreateValidItem();
        item.Type = "decimal";

        var results = Validate(item);

        Assert.Contains(results, x => x.MemberNames.Contains(nameof(ConfigurationItem.Type)));
    }

    private static ConfigurationItem CreateValidItem()
    {
        return new ConfigurationItem
        {
            Name = "SiteName",
            Type = "string",
            Value = "soty.io",
            IsActive = true,
            ApplicationName = "SERVICE-A"
        };
    }

    private static List<ValidationResult> Validate(ConfigurationItem item)
    {
        var context = new ValidationContext(item);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(
            item,
            context,
            results,
            validateAllProperties: true);

        return results;
    }
}