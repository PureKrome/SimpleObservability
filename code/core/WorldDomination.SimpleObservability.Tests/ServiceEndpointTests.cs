using Bogus;

namespace WorldDomination.SimpleObservability.Tests;

/// <summary>
/// Tests for the <see cref="ServiceEndpoint"/> record.
/// </summary>
public class ServiceEndpointTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Constructor_WithRequiredProperties_ShouldCreateInstance()
    {
        // Arrange.
        var name = _faker.Company.CompanyName();
        var environment = _faker.PickRandom("DEV", "UAT", "PROD");
        var healthCheckUrl = _faker.Internet.Url();

        // Act.
        var endpoint = new ServiceEndpoint
        {
            Name = name,
            Environment = environment,
            HealthCheckUrl = healthCheckUrl
        };

        // Assert.
        endpoint.Name.ShouldBe(name);
        endpoint.Environment.ShouldBe(environment);
        endpoint.HealthCheckUrl.ShouldBe(healthCheckUrl);
        endpoint.Description.ShouldBeNull();
        endpoint.Enabled.ShouldBeTrue();
        endpoint.TimeoutSeconds.ShouldBeNull();
    }

    [Fact]
    public void Constructor_WithAllProperties_ShouldCreateInstance()
    {
        // Arrange.
        var name = _faker.Company.CompanyName();
        var environment = _faker.PickRandom("DEV", "UAT", "PROD");
        var healthCheckUrl = _faker.Internet.Url();
        var description = _faker.Lorem.Sentence();
        var enabled = _faker.Random.Bool();
        var timeoutSeconds = _faker.Random.Int(1, 60);

        // Act.
        var endpoint = new ServiceEndpoint
        {
            Name = name,
            Environment = environment,
            HealthCheckUrl = healthCheckUrl,
            Description = description,
            Enabled = enabled,
            TimeoutSeconds = timeoutSeconds
        };

        // Assert.
        endpoint.Name.ShouldBe(name);
        endpoint.Environment.ShouldBe(environment);
        endpoint.HealthCheckUrl.ShouldBe(healthCheckUrl);
        endpoint.Description.ShouldBe(description);
        endpoint.Enabled.ShouldBe(enabled);
        endpoint.TimeoutSeconds.ShouldBe(timeoutSeconds);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Enabled_WithDifferentValues_ShouldSetCorrectly(bool enabled)
    {
        // Arrange.
        // Act.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = _faker.Internet.Url(),
            Enabled = enabled
        };

        // Assert.
        endpoint.Enabled.ShouldBe(enabled);
    }

    [Fact]
    public void Enabled_DefaultValue_ShouldBeTrue()
    {
        // Arrange.
        // Act.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = _faker.Internet.Url()
        };

        // Assert.
        endpoint.Enabled.ShouldBeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(30)]
    [InlineData(60)]
    public void TimeoutSeconds_WithDifferentValues_ShouldSetCorrectly(int timeout)
    {
        // Arrange.
        // Act.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = _faker.Internet.Url(),
            TimeoutSeconds = timeout
        };

        // Assert.
        endpoint.TimeoutSeconds.ShouldBe(timeout);
    }

    [Fact]
    public void Record_WithEqualValues_ShouldBeEqual()
    {
        // Arrange.
        var name = _faker.Company.CompanyName();
        var environment = "DEV";
        var url = _faker.Internet.Url();

        var endpoint1 = new ServiceEndpoint
        {
            Name = name,
            Environment = environment,
            HealthCheckUrl = url
        };

        var endpoint2 = new ServiceEndpoint
        {
            Name = name,
            Environment = environment,
            HealthCheckUrl = url
        };

        // Act.
        var areEqual = endpoint1 == endpoint2;

        // Assert.
        areEqual.ShouldBeTrue();
        endpoint1.GetHashCode().ShouldBe(endpoint2.GetHashCode());
    }

    [Fact]
    public void Record_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange.
        var endpoint1 = new ServiceEndpoint
        {
            Name = "Service A",
            Environment = "DEV",
            HealthCheckUrl = _faker.Internet.Url()
        };

        var endpoint2 = new ServiceEndpoint
        {
            Name = "Service B",
            Environment = "DEV",
            HealthCheckUrl = _faker.Internet.Url()
        };

        // Act.
        var areEqual = endpoint1 == endpoint2;

        // Assert.
        areEqual.ShouldBeFalse();
    }

    [Fact]
    public void HealthCheckUrl_WithHttpsUrl_ShouldStoreCorrectly()
    {
        // Arrange.
        var url = "https://api.example.com/healthz";

        // Act.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "PROD",
            HealthCheckUrl = url
        };

        // Assert.
        endpoint.HealthCheckUrl.ShouldBe(url);
    }

    [Fact]
    public void HealthCheckUrl_WithHttpUrl_ShouldStoreCorrectly()
    {
        // Arrange.
        var url = "http://localhost:5000/healthz";

        // Act.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = url
        };

        // Assert.
        endpoint.HealthCheckUrl.ShouldBe(url);
    }
}
