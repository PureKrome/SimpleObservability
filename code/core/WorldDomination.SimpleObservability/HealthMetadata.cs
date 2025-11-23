namespace WorldDomination.SimpleObservability;

/// <summary>
/// Represents the standard health metadata schema for service observability.
/// This is the POCO that should be returned from your /healthz endpoint.
/// </summary>
/// <remarks>
/// Example usage in your API:
/// <code>
/// app.MapGet("/healthz", () =>
/// {
///     var metadata = new HealthMetadata
///     {
///         ServiceName = "My Service",
///         Version = "1.2.3",
///         Environment = "Production",
///         Status = HealthStatus.Healthy
///     };
///     return Results.Json(metadata);
/// });
/// </code>
/// </remarks>
public record HealthMetadata
{
    /// <summary>
    /// The name of the service. Required.
    /// </summary>
    /// <example>My API Service</example>
    public required string ServiceName { get; init; }

    /// <summary>
    /// The version of the service. Can be a semantic version (1.2.3), git branch name, commit hash, or any identifier. Required.
    /// </summary>
    /// <example>1.2.3.4</example>
    /// <example>feature/new-dashboard</example>
    /// <example>abc123def</example>
    public required string Version { get; init; }

    /// <summary>
    /// The environment this service is running in. Optional.
    /// </summary>
    /// <example>Development</example>
    /// <example>UAT</example>
    /// <example>Production</example>
    public string? Environment { get; init; }

    /// <summary>
    /// The current health status of the service. Optional, defaults to Healthy.
    /// </summary>
    public HealthStatus Status { get; init; } = HealthStatus.Healthy;

    /// <summary>
    /// The timestamp when this health check was performed. Optional, defaults to current UTC time.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Additional custom metadata about the service. Optional.
    /// Use this for any service-specific information you want to expose.
    /// </summary>
    /// <example>
    /// new Dictionary&lt;string, string&gt;
    /// {
    ///     ["Database"] = "Connected",
    ///     ["Cache"] = "Redis v7.0",
    ///     ["HostName"] = "server-01"
    /// }
    /// </example>
    public Dictionary<string, string>? AdditionalMetadata { get; init; }

    /// <summary>
    /// Description or additional details about the current status. Optional.
    /// </summary>
    /// <example>All systems operational</example>
    /// <example>Database connection degraded</example>
    public string? Description { get; init; }

    /// <summary>
    /// The hostname or machine name where the service is running. Optional.
    /// </summary>
    public string? HostName { get; init; }

    /// <summary>
    /// The uptime of the service since last restart. Optional.
    /// </summary>
    public TimeSpan? Uptime { get; init; }
}
