namespace WorldDomination.SimpleObservability.Tests;

/// <summary>
/// Tests for the <see cref="HealthStatus"/> enum.
/// </summary>
public class HealthStatusTests
{
    [Fact]
    public void HealthStatus_ShouldHaveThreeValues()
    {
        // Arrange.
        // Act.
        var values = Enum.GetValues<HealthStatus>();

        // Assert.
        values.Length.ShouldBe(3);
    }

    [Fact]
    public void HealthStatus_ShouldContainHealthy()
    {
        // Arrange.
        // Act.
        var hasHealthy = Enum.IsDefined(typeof(HealthStatus), HealthStatus.Healthy);

        // Assert.
        hasHealthy.ShouldBeTrue();
    }

    [Fact]
    public void HealthStatus_ShouldContainDegraded()
    {
        // Arrange.
        // Act.
        var hasDegraded = Enum.IsDefined(typeof(HealthStatus), HealthStatus.Degraded);

        // Assert.
        hasDegraded.ShouldBeTrue();
    }

    [Fact]
    public void HealthStatus_ShouldContainUnhealthy()
    {
        // Arrange.
        // Act.
        var hasUnhealthy = Enum.IsDefined(typeof(HealthStatus), HealthStatus.Unhealthy);

        // Assert.
        hasUnhealthy.ShouldBeTrue();
    }

    [Theory]
    [InlineData(HealthStatus.Healthy, 0)]
    [InlineData(HealthStatus.Degraded, 1)]
    [InlineData(HealthStatus.Unhealthy, 2)]
    public void HealthStatus_ShouldHaveCorrectIntegerValues(HealthStatus status, int expectedValue)
    {
        // Arrange.
        // Act.
        var actualValue = (int)status;

        // Assert.
        actualValue.ShouldBe(expectedValue);
    }

    [Theory]
    [InlineData(0, HealthStatus.Healthy)]
    [InlineData(1, HealthStatus.Degraded)]
    [InlineData(2, HealthStatus.Unhealthy)]
    public void HealthStatus_ShouldCastFromInteger(int value, HealthStatus expected)
    {
        // Arrange.
        // Act.
        var status = (HealthStatus)value;

        // Assert.
        status.ShouldBe(expected);
    }

    [Fact]
    public void HealthStatus_ToString_ShouldReturnName()
    {
        // Arrange.
        // Act.
        var healthyName = HealthStatus.Healthy.ToString();
        var degradedName = HealthStatus.Degraded.ToString();
        var unhealthyName = HealthStatus.Unhealthy.ToString();

        // Assert.
        healthyName.ShouldBe("Healthy");
        degradedName.ShouldBe("Degraded");
        unhealthyName.ShouldBe("Unhealthy");
    }
}
