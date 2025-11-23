namespace WorldDomination.SimpleObservability;

/// <summary>
/// Configuration for the Simple Observability dashboard.
/// </summary>
public record DashboardConfiguration
{
    /// <summary>
    /// List of services to monitor. Each service will appear as a column in the dashboard. Required.
    /// The environments are automatically derived from the unique environment values in the services list.
    /// </summary>
    public required List<ServiceEndpoint> Services { get; init; }

    /// <summary>
    /// The interval (in seconds) at which the dashboard should refresh health data. Optional, defaults to 30 seconds.
    /// </summary>
    public int RefreshIntervalSeconds { get; init; } = 30;

    /// <summary>
    /// Timeout (in seconds) for health check requests. Optional, defaults to 5 seconds.
    /// This is the default timeout; individual services can override this with their own timeout.
    /// </summary>
    public int TimeoutSeconds { get; init; } = 5;

    /// <summary>
    /// Optional ordered list of environment names. Environments will be displayed in this order.
    /// Any environments found in services but not listed here will appear last, sorted alphabetically.
    /// </summary>
    /// <example>["DEV", "UAT", "PROD"]</example>
    public List<string>? EnvironmentOrder { get; init; }

    /// <summary>
    /// Gets the list of unique environments from the services.
    /// Environments are ordered according to EnvironmentOrder if specified.
    /// Unlisted environments appear last in alphabetical order.
    /// </summary>
    public List<string> Environments
    {
        get
        {
            var allEnvironments = Services
                .Select(s => s.Environment)
                .Distinct()
                .ToList();

            if (EnvironmentOrder is null || EnvironmentOrder.Count == 0)
            {
                // No custom order specified, return alphabetically sorted.
                return allEnvironments.OrderBy(e => e).ToList();
            }

            // Separate environments into ordered and unordered groups.
            var orderedEnvironments = new List<string>();
            var unorderedEnvironments = new List<string>();

            foreach (var env in allEnvironments)
            {
                if (EnvironmentOrder.Contains(env))
                {
                    orderedEnvironments.Add(env);
                }
                else
                {
                    unorderedEnvironments.Add(env);
                }
            }

            // Sort ordered environments by their position in EnvironmentOrder.
            orderedEnvironments = orderedEnvironments
                .OrderBy(e => EnvironmentOrder.IndexOf(e))
                .ToList();

            // Sort unordered environments alphabetically.
            unorderedEnvironments = unorderedEnvironments
                .OrderBy(e => e)
                .ToList();

            // Combine: ordered first, then unordered.
            return orderedEnvironments.Concat(unorderedEnvironments).ToList();
        }
    }
}
