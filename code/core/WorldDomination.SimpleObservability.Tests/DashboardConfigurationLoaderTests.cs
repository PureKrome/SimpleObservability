using Microsoft.Extensions.Configuration;

namespace WorldDomination.SimpleObservability.Tests;

/// <summary>
/// Tests for the <see cref="DashboardConfigurationLoader"/> class.
/// </summary>
public class DashboardConfigurationLoaderTests
{
    [Fact]
    public void Load_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Arrange.
        IConfiguration configuration = null!;

        // Act.
        var exception = Should.Throw<ArgumentNullException>(() => configuration.Load());

        // Assert.
        exception.ParamName.ShouldBe("configuration");
    }

    [Fact]
    public void Load_WithMissingDashboardSection_ShouldReturnDefaultConfiguration()
    {
        // Arrange.
        var configuration = new ConfigurationBuilder().Build();

        // Act.
        var config = configuration.Load();

        // Assert.
        config.ShouldNotBeNull();
        config.Services.ShouldNotBeNull();
        config.Services.ShouldBeEmpty();
        config.RefreshIntervalSeconds.ShouldBe(30);
        config.TimeoutSeconds.ShouldBe(5);
        config.EnvironmentOrder.ShouldBeNull();
    }

    [Fact]
    public void Load_WithValidDashboardSection_ShouldLoadConfiguration()
    {
        // Arrange.
        var configData = new Dictionary<string, string?>
        {
            ["Dashboard:Services:0:Name"] = "Test Service",
            ["Dashboard:Services:0:Environment"] = "DEV",
            ["Dashboard:Services:0:HealthCheckUrl"] = "http://localhost:5000/healthz",
            ["Dashboard:Services:0:Description"] = "Test service description",
            ["Dashboard:Services:0:Enabled"] = "true",
            ["Dashboard:RefreshIntervalSeconds"] = "60",
            ["Dashboard:TimeoutSeconds"] = "10",
            ["Dashboard:EnvironmentOrder:0"] = "PROD",
            ["Dashboard:EnvironmentOrder:1"] = "UAT",
            ["Dashboard:EnvironmentOrder:2"] = "DEV"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act.
        var config = configuration.Load();

        // Assert.
        config.ShouldNotBeNull();
        config.Services.Count.ShouldBe(1);
        config.Services[0].Name.ShouldBe("Test Service");
        config.Services[0].Environment.ShouldBe("DEV");
        config.Services[0].HealthCheckUrl.ShouldBe("http://localhost:5000/healthz");
        config.Services[0].Description.ShouldBe("Test service description");
        config.Services[0].Enabled.ShouldBeTrue();
        config.RefreshIntervalSeconds.ShouldBe(60);
        config.TimeoutSeconds.ShouldBe(10);
        config.EnvironmentOrder.ShouldNotBeNull();
        config.EnvironmentOrder!.Count.ShouldBe(3);
        config.EnvironmentOrder[0].ShouldBe("PROD");
        config.EnvironmentOrder[1].ShouldBe("UAT");
        config.EnvironmentOrder[2].ShouldBe("DEV");
    }

    [Fact]
    public void Load_WithMultipleServices_ShouldLoadAllServices()
    {
        // Arrange.
        var configData = new Dictionary<string, string?>
        {
            ["Dashboard:Services:0:Name"] = "Service 1",
            ["Dashboard:Services:0:Environment"] = "DEV",
            ["Dashboard:Services:0:HealthCheckUrl"] = "http://localhost:5001/healthz",
            ["Dashboard:Services:1:Name"] = "Service 2",
            ["Dashboard:Services:1:Environment"] = "UAT",
            ["Dashboard:Services:1:HealthCheckUrl"] = "http://localhost:5002/healthz",
            ["Dashboard:Services:2:Name"] = "Service 3",
            ["Dashboard:Services:2:Environment"] = "PROD",
            ["Dashboard:Services:2:HealthCheckUrl"] = "http://localhost:5003/healthz"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act.
        var config = configuration.Load();

        // Assert.
        config.Services.Count.ShouldBe(3);
        config.Services[0].Name.ShouldBe("Service 1");
        config.Services[0].Environment.ShouldBe("DEV");
        config.Services[1].Name.ShouldBe("Service 2");
        config.Services[1].Environment.ShouldBe("UAT");
        config.Services[2].Name.ShouldBe("Service 3");
        config.Services[2].Environment.ShouldBe("PROD");
    }

    [Fact]
    public void Load_WithCustomSectionName_ShouldLoadFromCustomSection()
    {
        // Arrange.
        var configData = new Dictionary<string, string?>
        {
            ["CustomSection:Services:0:Name"] = "Test Service",
            ["CustomSection:Services:0:Environment"] = "DEV",
            ["CustomSection:Services:0:HealthCheckUrl"] = "http://localhost:5000/healthz",
            ["CustomSection:RefreshIntervalSeconds"] = "45"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act.
        var config = configuration.Load("CustomSection");

        // Assert.
        config.Services.Count.ShouldBe(1);
        config.Services[0].Name.ShouldBe("Test Service");
        config.RefreshIntervalSeconds.ShouldBe(45);
    }

    [Fact]
    public void Load_WithServiceTimeouts_ShouldLoadTimeouts()
    {
        // Arrange.
        var configData = new Dictionary<string, string?>
        {
            ["Dashboard:Services:0:Name"] = "Test Service",
            ["Dashboard:Services:0:Environment"] = "DEV",
            ["Dashboard:Services:0:HealthCheckUrl"] = "http://localhost:5000/healthz",
            ["Dashboard:Services:0:TimeoutSeconds"] = "15"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act.
        var config = configuration.Load();

        // Assert.
        config.Services[0].TimeoutSeconds.ShouldBe(15);
    }

    [Fact]
    public void Load_WithDisabledService_ShouldLoadEnabledProperty()
    {
        // Arrange.
        var configData = new Dictionary<string, string?>
        {
            ["Dashboard:Services:0:Name"] = "Test Service",
            ["Dashboard:Services:0:Environment"] = "DEV",
            ["Dashboard:Services:0:HealthCheckUrl"] = "http://localhost:5000/healthz",
            ["Dashboard:Services:0:Enabled"] = "false"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act.
        var config = configuration.Load();

        // Assert.
        config.Services[0].Enabled.ShouldBeFalse();
    }

    [Fact]
    public void CreateDefaultConfiguration_ShouldReturnValidDefaultConfiguration()
    {
        // Arrange.
        // Act.
        var config = DashboardConfigurationLoader.CreateDefaultConfiguration();

        // Assert.
        config.ShouldNotBeNull();
        config.Services.ShouldNotBeNull();
        config.Services.ShouldBeEmpty();
        config.RefreshIntervalSeconds.ShouldBe(30);
        config.TimeoutSeconds.ShouldBe(5);
        config.EnvironmentOrder.ShouldBeNull();
    }

    [Fact]
    public void AddDashboardSettingsFile_WithNullBuilder_ShouldThrowArgumentNullException()
    {
        // Arrange.
        IConfigurationBuilder builder = null!;

        // Act.
        var exception = Should.Throw<ArgumentNullException>(() => builder.AddDashboardSettingsFile());

        // Assert.
        exception.ParamName.ShouldBe("builder");
    }

    [Fact]
    public void AddDashboardSettingsFile_WithDefaultFilename_ShouldAddJsonFile()
    {
        // Arrange.
        var builder = new ConfigurationBuilder();

        // Act.
        var result = builder.AddDashboardSettingsFile();

        // Assert.
        result.ShouldBe(builder);
        result.ShouldNotBeNull();
    }

    [Fact]
    public void AddDashboardSettingsFile_WithCustomFilename_ShouldAddJsonFile()
    {
        // Arrange.
        var builder = new ConfigurationBuilder();
        var customFilename = "custom-settings.json";

        // Act.
        var result = builder.AddDashboardSettingsFile(customFilename);

        // Assert.
        result.ShouldBe(builder);
        result.ShouldNotBeNull();
    }

    [Fact]
    public void AddDashboardSettingsFile_WithOptionalFalse_ShouldAddJsonFile()
    {
        // Arrange.
        var builder = new ConfigurationBuilder();

        // Act.
        var result = builder.AddDashboardSettingsFile(optional: false);

        // Assert.
        result.ShouldBe(builder);
        result.ShouldNotBeNull();
    }

    [Fact]
    public void DefaultSectionName_ShouldBeDashboard()
    {
        // Arrange.
        // Act.
        var sectionName = DashboardConfigurationLoader.DefaultSectionName;

        // Assert.
        sectionName.ShouldBe("Dashboard");
    }
}
