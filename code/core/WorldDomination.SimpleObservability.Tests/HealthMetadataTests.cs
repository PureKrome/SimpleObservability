using Bogus;

namespace WorldDomination.SimpleObservability.Tests;

/// <summary>
/// Tests for the <see cref="HealthMetadata"/> record.
/// </summary>
public class HealthMetadataTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Constructor_WithRequiredProperties_ShouldCreateInstance()
    {
        // Arrange.
        var serviceName = _faker.Company.CompanyName();
        var version = _faker.System.Version().ToString();

        // Act.
        var metadata = new HealthMetadata
        {
            ServiceName = serviceName,
            Version = version
        };

        // Assert.
        metadata.ServiceName.ShouldBe(serviceName);
        metadata.Version.ShouldBe(version);
        metadata.Status.ShouldBe(HealthStatus.Healthy);
        metadata.Timestamp.ShouldBeInRange(DateTimeOffset.UtcNow.AddSeconds(-5), DateTimeOffset.UtcNow.AddSeconds(1));
        metadata.Environment.ShouldBeNull();
        metadata.AdditionalMetadata.ShouldBeNull();
        metadata.Description.ShouldBeNull();
        metadata.HostName.ShouldBeNull();
        metadata.Uptime.ShouldBeNull();
    }

    [Fact]
    public void Constructor_WithAllProperties_ShouldCreateInstance()
    {
        // Arrange.
        var serviceName = _faker.Company.CompanyName();
        var version = _faker.System.Version().ToString();
        var environment = _faker.PickRandom("DEV", "UAT", "PROD");
        var status = _faker.PickRandom<HealthStatus>();
        var timestamp = DateTimeOffset.UtcNow;
        var additionalMetadata = new Dictionary<string, string>
        {
            ["Database"] = "Connected",
            ["Cache"] = "Redis v7.0"
        };
        var description = _faker.Lorem.Sentence();
        var hostName = _faker.Internet.DomainName();
        var uptime = TimeSpan.FromHours(_faker.Random.Double(1, 1000));

        // Act.
        var metadata = new HealthMetadata
        {
            ServiceName = serviceName,
            Version = version,
            Environment = environment,
            Status = status,
            Timestamp = timestamp,
            AdditionalMetadata = additionalMetadata,
            Description = description,
            HostName = hostName,
            Uptime = uptime
        };

        // Assert.
        metadata.ServiceName.ShouldBe(serviceName);
        metadata.Version.ShouldBe(version);
        metadata.Environment.ShouldBe(environment);
        metadata.Status.ShouldBe(status);
        metadata.Timestamp.ShouldBe(timestamp);
        metadata.AdditionalMetadata.ShouldBe(additionalMetadata);
        metadata.Description.ShouldBe(description);
        metadata.HostName.ShouldBe(hostName);
        metadata.Uptime.ShouldBe(uptime);
    }

    [Theory]
    [InlineData(HealthStatus.Healthy)]
    [InlineData(HealthStatus.Degraded)]
    [InlineData(HealthStatus.Unhealthy)]
    public void Status_WithDifferentStatuses_ShouldSetCorrectly(HealthStatus status)
    {
        // Arrange.
        // Act.
        var metadata = new HealthMetadata
        {
            ServiceName = _faker.Company.CompanyName(),
            Version = _faker.System.Version().ToString(),
            Status = status
        };

        // Assert.
        metadata.Status.ShouldBe(status);
    }

    [Fact]
    public void Timestamp_DefaultValue_ShouldBeApproximatelyNow()
    {
        // Arrange.
        var before = DateTimeOffset.UtcNow;

        // Act.
        var metadata = new HealthMetadata
        {
            ServiceName = _faker.Company.CompanyName(),
            Version = _faker.System.Version().ToString()
        };

        // Assert.
        var after = DateTimeOffset.UtcNow;
        metadata.Timestamp.ShouldBeInRange(before, after);
    }

    [Fact]
    public void AdditionalMetadata_WithMultipleEntries_ShouldStoreCorrectly()
    {
        // Arrange.
        var additionalMetadata = new Dictionary<string, string>
        {
            ["Database"] = "PostgreSQL",
            ["Cache"] = "Redis",
            ["Queue"] = "RabbitMQ",
            ["Storage"] = "Azure Blob"
        };

        // Act.
        var metadata = new HealthMetadata
        {
            ServiceName = _faker.Company.CompanyName(),
            Version = _faker.System.Version().ToString(),
            AdditionalMetadata = additionalMetadata
        };

        // Assert.
        metadata.AdditionalMetadata.ShouldNotBeNull();
        metadata.AdditionalMetadata.Count.ShouldBe(4);
        metadata.AdditionalMetadata["Database"].ShouldBe("PostgreSQL");
        metadata.AdditionalMetadata["Cache"].ShouldBe("Redis");
        metadata.AdditionalMetadata["Queue"].ShouldBe("RabbitMQ");
        metadata.AdditionalMetadata["Storage"].ShouldBe("Azure Blob");
    }

    [Fact]
    public void Record_WithEqualValues_ShouldBeEqual()
    {
        // Arrange.
        var serviceName = _faker.Company.CompanyName();
        var version = _faker.System.Version().ToString();
        var timestamp = DateTimeOffset.UtcNow;

        var metadata1 = new HealthMetadata
        {
            ServiceName = serviceName,
            Version = version,
            Timestamp = timestamp
        };

        var metadata2 = new HealthMetadata
        {
            ServiceName = serviceName,
            Version = version,
            Timestamp = timestamp
        };

        // Act.
        var areEqual = metadata1 == metadata2;

        // Assert.
        areEqual.ShouldBeTrue();
        metadata1.GetHashCode().ShouldBe(metadata2.GetHashCode());
    }

    [Fact]
    public void Record_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange.
        var metadata1 = new HealthMetadata
        {
            ServiceName = _faker.Company.CompanyName(),
            Version = "1.0.0"
        };

        var metadata2 = new HealthMetadata
        {
            ServiceName = _faker.Company.CompanyName(),
            Version = "2.0.0"
        };

        // Act.
        var areEqual = metadata1 == metadata2;

        // Assert.
        areEqual.ShouldBeFalse();
    }
}
