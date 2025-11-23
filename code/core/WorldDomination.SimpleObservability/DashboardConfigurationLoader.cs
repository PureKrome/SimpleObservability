using Microsoft.Extensions.Configuration;

namespace WorldDomination.SimpleObservability;

/// <summary>
/// Helper class to load dashboard configuration from various sources.
/// </summary>
public static class DashboardConfigurationLoader
{
    public const string DefaultSectionName = "Dashboard";

    /// <summary>
    /// Loads dashboard configuration from the provided configuration source.
    /// </summary>
    /// <param name="configuration">The configuration source to read from.</param>
    /// <param name="sectionName">The section name to read from (default: "Dashboard").</param>
    /// <returns>The dashboard configuration, or a default configuration if not found.</returns>
    public static DashboardConfiguration Load(this IConfiguration configuration, string sectionName = DefaultSectionName)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var config = configuration.GetSection(sectionName).Get<DashboardConfiguration>();

        // Return default configuration if not found.
        if (config is null)
        {
            return CreateDefaultConfiguration();
        }

        return config;
    }

    /// <summary>
    /// Creates a default dashboard configuration.
    /// </summary>
    /// <returns>A default dashboard configuration with no services.</returns>
    public static DashboardConfiguration CreateDefaultConfiguration()
    {
        return new DashboardConfiguration
        {
            Services = new List<ServiceEndpoint>(),
            RefreshIntervalSeconds = 30,
            TimeoutSeconds = 5
        };
    }

    /// <summary>
    /// Adds an optional dashboard settings JSON file to the configuration builder.
    /// </summary>
    /// <param name="builder">The configuration builder.</param>
    /// <param name="filename">The filename to load (default: "dashboardsettings.json").</param>
    /// <param name="optional">Whether the file is optional (default: true).</param>
    /// <param name="reloadOnChange">Whether to reload the configuration when the file changes (default: true).</param>
    /// <returns>The configuration builder for chaining.</returns>
    public static IConfigurationBuilder AddDashboardSettingsFile(
        this IConfigurationBuilder builder,
        string filename = "dashboardsettings.json",
        bool optional = true,
        bool reloadOnChange = true)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddJsonFile(filename, optional: optional, reloadOnChange: reloadOnChange);
    }
}
