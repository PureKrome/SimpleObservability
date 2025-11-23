using Bogus;

namespace WorldDomination.SimpleObservability.Tests;

/// <summary>
/// Tests for the <see cref="DashboardConfiguration"/> record.
/// </summary>
public class DashboardConfigurationTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Constructor_WithRequiredProperties_ShouldCreateInstance()
    {
        // Arrange.
        var services = new List<ServiceEndpoint>
        {
            new()
            {
                Name = _faker.Company.CompanyName(),
                Environment = "DEV",
                HealthCheckUrl = _faker.Internet.Url()
            }
        };

        // Act.
        var config = new DashboardConfiguration
        {
            Services = services
        };

        // Assert.
        config.Services.ShouldBe(services);
        config.RefreshIntervalSeconds.ShouldBe(30);
        config.TimeoutSeconds.ShouldBe(5);
        config.EnvironmentOrder.ShouldBeNull();
    }

    [Fact]
    public void Constructor_WithAllProperties_ShouldCreateInstance()
    {
        // Arrange.
        var services = new List<ServiceEndpoint>
        {
            new()
            {
                Name = _faker.Company.CompanyName(),
                Environment = "DEV",
                HealthCheckUrl = _faker.Internet.Url()
            }
        };
        var refreshInterval = _faker.Random.Int(10, 120);
        var timeout = _faker.Random.Int(1, 30);
        var environmentOrder = new List<string> { "PROD", "UAT", "DEV" };

        // Act.
        var config = new DashboardConfiguration
        {
            Services = services,
            RefreshIntervalSeconds = refreshInterval,
            TimeoutSeconds = timeout,
            EnvironmentOrder = environmentOrder
        };

        // Assert.
        config.Services.ShouldBe(services);
        config.RefreshIntervalSeconds.ShouldBe(refreshInterval);
        config.TimeoutSeconds.ShouldBe(timeout);
        config.EnvironmentOrder.ShouldBe(environmentOrder);
    }

    [Fact]
    public void Environments_WithNoServices_ShouldReturnEmptyList()
    {
        // Arrange.
        var config = new DashboardConfiguration
        {
            Services = new List<ServiceEndpoint>()
        };

        // Act.
        var environments = config.Environments;

        // Assert.
        environments.ShouldBeEmpty();
    }

    [Fact]
    public void Environments_WithMultipleServices_ShouldReturnUniqueEnvironments()
    {
        // Arrange.
        var config = new DashboardConfiguration
        {
            Services = new List<ServiceEndpoint>
            {
                new() { Name = "Service 1", Environment = "DEV", HealthCheckUrl = "http://localhost:5001" },
                new() { Name = "Service 2", Environment = "DEV", HealthCheckUrl = "http://localhost:5002" },
                new() { Name = "Service 3", Environment = "UAT", HealthCheckUrl = "http://localhost:5003" },
                new() { Name = "Service 4", Environment = "PROD", HealthCheckUrl = "http://localhost:5004" }
            }
        };

        // Act.
        var environments = config.Environments;

        // Assert.
        environments.Count.ShouldBe(3);
        environments.ShouldContain("DEV");
        environments.ShouldContain("UAT");
        environments.ShouldContain("PROD");
    }

    [Fact]
    public void Environments_WithNoEnvironmentOrder_ShouldReturnAlphabeticallySorted()
    {
        // Arrange.
        var config = new DashboardConfiguration
        {
            Services = new List<ServiceEndpoint>
            {
                new() { Name = "Service 1", Environment = "PROD", HealthCheckUrl = "http://localhost:5001" },
                new() { Name = "Service 2", Environment = "DEV", HealthCheckUrl = "http://localhost:5002" },
                new() { Name = "Service 3", Environment = "UAT", HealthCheckUrl = "http://localhost:5003" }
            }
        };

        // Act.
        var environments = config.Environments;

        // Assert.
        environments.Count.ShouldBe(3);
        environments[0].ShouldBe("DEV");
        environments[1].ShouldBe("PROD");
        environments[2].ShouldBe("UAT");
    }

    [Fact]
    public void Environments_WithEnvironmentOrder_ShouldRespectOrder()
    {
        // Arrange.
        var config = new DashboardConfiguration
        {
            Services = new List<ServiceEndpoint>
            {
                new() { Name = "Service 1", Environment = "PROD", HealthCheckUrl = "http://localhost:5001" },
                new() { Name = "Service 2", Environment = "DEV", HealthCheckUrl = "http://localhost:5002" },
                new() { Name = "Service 3", Environment = "UAT", HealthCheckUrl = "http://localhost:5003" }
            },
            EnvironmentOrder = new List<string> { "PROD", "UAT", "DEV" }
        };

        // Act.
        var environments = config.Environments;

        // Assert.
        environments.Count.ShouldBe(3);
        environments[0].ShouldBe("PROD");
        environments[1].ShouldBe("UAT");
        environments[2].ShouldBe("DEV");
    }

    [Fact]
    public void Environments_WithPartialEnvironmentOrder_ShouldPlaceUnorderedLast()
    {
        // Arrange.
        var config = new DashboardConfiguration
        {
            Services = new List<ServiceEndpoint>
            {
                new() { Name = "Service 1", Environment = "PROD", HealthCheckUrl = "http://localhost:5001" },
                new() { Name = "Service 2", Environment = "DEV", HealthCheckUrl = "http://localhost:5002" },
                new() { Name = "Service 3", Environment = "UAT", HealthCheckUrl = "http://localhost:5003" },
                new() { Name = "Service 4", Environment = "STAGING", HealthCheckUrl = "http://localhost:5004" }
            },
            EnvironmentOrder = new List<string> { "PROD", "UAT" }
        };

        // Act.
        var environments = config.Environments;

        // Assert.
        environments.Count.ShouldBe(4);
        environments[0].ShouldBe("PROD");
        environments[1].ShouldBe("UAT");
        environments[2].ShouldBe("DEV");
        environments[3].ShouldBe("STAGING");
    }

    [Fact]
    public void Environments_WithEmptyEnvironmentOrder_ShouldReturnAlphabeticallySorted()
    {
        // Arrange.
        var config = new DashboardConfiguration
        {
            Services = new List<ServiceEndpoint>
            {
                new() { Name = "Service 1", Environment = "PROD", HealthCheckUrl = "http://localhost:5001" },
                new() { Name = "Service 2", Environment = "DEV", HealthCheckUrl = "http://localhost:5002" },
                new() { Name = "Service 3", Environment = "UAT", HealthCheckUrl = "http://localhost:5003" }
            },
            EnvironmentOrder = new List<string>()
        };

        // Act.
        var environments = config.Environments;

        // Assert.
        environments.Count.ShouldBe(3);
        environments[0].ShouldBe("DEV");
        environments[1].ShouldBe("PROD");
        environments[2].ShouldBe("UAT");
    }

    [Fact]
    public void Environments_WithDuplicateEnvironments_ShouldReturnUniqueOnly()
    {
        // Arrange.
        var config = new DashboardConfiguration
        {
            Services = new List<ServiceEndpoint>
            {
                new() { Name = "Service 1", Environment = "DEV", HealthCheckUrl = "http://localhost:5001" },
                new() { Name = "Service 2", Environment = "DEV", HealthCheckUrl = "http://localhost:5002" },
                new() { Name = "Service 3", Environment = "DEV", HealthCheckUrl = "http://localhost:5003" }
            }
        };

        // Act.
        var environments = config.Environments;

        // Assert.
        environments.Count.ShouldBe(1);
        environments[0].ShouldBe("DEV");
    }

    [Fact]
    public void RefreshIntervalSeconds_DefaultValue_ShouldBe30()
    {
        // Arrange.
        // Act.
        var config = new DashboardConfiguration
        {
            Services = new List<ServiceEndpoint>()
        };

        // Assert.
        config.RefreshIntervalSeconds.ShouldBe(30);
    }

    [Fact]
    public void TimeoutSeconds_DefaultValue_ShouldBe5()
    {
        // Arrange.
        // Act.
        var config = new DashboardConfiguration
        {
            Services = new List<ServiceEndpoint>()
        };

        // Assert.
        config.TimeoutSeconds.ShouldBe(5);
    }

    [Fact]
    public void Record_WithEqualValues_ShouldBeEqual()
    {
        // Arrange.
        var services = new List<ServiceEndpoint>
        {
            new() { Name = "Service 1", Environment = "DEV", HealthCheckUrl = "http://localhost:5001" }
        };

        var config1 = new DashboardConfiguration
        {
            Services = services,
            RefreshIntervalSeconds = 60,
            TimeoutSeconds = 10
        };

        var config2 = new DashboardConfiguration
        {
            Services = services,
            RefreshIntervalSeconds = 60,
            TimeoutSeconds = 10
        };

        // Act.
        var areEqual = config1 == config2;

        // Assert.
        areEqual.ShouldBeTrue();
    }
}
