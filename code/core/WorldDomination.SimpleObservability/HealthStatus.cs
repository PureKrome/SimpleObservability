namespace WorldDomination.SimpleObservability;

/// <summary>
/// Represents the health status of a service.
/// </summary>
public enum HealthStatus
{
    /// <summary>
    /// The service is healthy and operating normally.
    /// </summary>
    Healthy,

    /// <summary>
    /// The service is operational but experiencing degraded performance or partial functionality.
    /// </summary>
    Degraded,

    /// <summary>
    /// The service is unhealthy and not operating correctly.
    /// </summary>
    Unhealthy
}
