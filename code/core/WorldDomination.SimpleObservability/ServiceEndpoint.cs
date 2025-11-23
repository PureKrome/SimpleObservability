namespace WorldDomination.SimpleObservability;

/// <summary>
/// Represents a service endpoint to monitor.
/// </summary>
public record ServiceEndpoint
{
    /// <summary>
    /// The display name for this service. Required.
    /// </summary>
    /// <example>Payment API</example>
    public required string Name { get; init; }

    /// <summary>
    /// The environment this service belongs to (e.g., DEV, UAT, PROD). Required.
    /// </summary>
    /// <example>PROD</example>
    public required string Environment { get; init; }

    /// <summary>
    /// The full URL to the health check endpoint. Required.
    /// </summary>
    /// <example>https://api.example.com/healthz</example>
    public required string HealthCheckUrl { get; init; }

    /// <summary>
    /// Optional description of this service.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Whether this service is enabled for monitoring. Optional, defaults to true.
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Optional timeout in seconds for this specific service. If not set, uses the system default timeout.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
