# Simple Observability - Health Metadata Schema

## Overview

The Simple Observability health metadata schema is a standardized JSON format for exposing service health and version information. Services that implement this schema can be monitored by the Simple Observability Dashboard.

## Schema Definition

### Required Fields

| Field | Type | Description | Example |
|-------|------|-------------|---------|
| `serviceName` | string | The name of your service | `"Payment API"` |
| `version` | string | Version identifier (semantic version, git branch, commit hash, etc.) | `"1.2.3"`, `"feature/new-feature"`, `"abc123"` |

### Optional Fields

| Field | Type | Description | Example |
|-------|------|-------------|---------|
| `environment` | string | Environment where service is running | `"Production"`, `"UAT"`, `"DEV"` |
| `status` | string | Health status: `"healthy"`, `"degraded"`, or `"unhealthy"` | `"healthy"` |
| `timestamp` | string (ISO 8601) | When health check was performed | `"2024-01-15T10:30:00Z"` |
| `description` | string | Additional details about the current status | `"All systems operational"` |
| `hostName` | string | Hostname or machine name | `"web-server-01"` |
| `uptime` | string (ISO 8601 duration) | Time since service started | `"P1DT2H30M"` |
| `additionalMetadata` | object | Key-value pairs of custom metadata | `{"database": "Connected", "cache": "Redis"}` |

## JSON Examples

### Minimal Example

```json
{
  "serviceName": "My API",
  "version": "1.0.0"
}
```

### Complete Example

```json
{
  "serviceName": "Payment API",
  "version": "2.3.1",
  "environment": "Production",
  "status": "healthy",
  "timestamp": "2024-01-15T10:30:00Z",
  "description": "All systems operational",
  "hostName": "payment-api-01",
  "uptime": "P7DT5H30M",
  "additionalMetadata": {
    "database": "PostgreSQL 15.2 - Connected",
    "cache": "Redis 7.0 - Connected",
    "queueDepth": "42",
    "region": "us-west-2"
  }
}
```

### Example with Git Branch as Version

```json
{
  "serviceName": "User Service",
  "version": "feature/new-authentication",
  "environment": "DEV",
  "status": "degraded",
  "description": "Testing new authentication flow",
  "additionalMetadata": {
    "commit": "abc123def456",
    "branch": "feature/new-authentication"
  }
}
```

## Health Status Values

| Value | Description |
|-------|-------------|
| `"healthy"` | Service is operating normally |
| `"degraded"` | Service is operational but experiencing issues |
| `"unhealthy"` | Service is not functioning correctly |

**Note:** Status values are case-insensitive when consumed by the dashboard.

## Implementing in Your Service

### Using the NuGet Package (.NET)

1. Install the NuGet package:
   ```bash
   dotnet add package WorldDomination.SimpleObservability
   ```

2. Create a `/healthz` endpoint:

   ```csharp
   using WorldDomination.SimpleObservability;
   
   app.MapGet("/healthz", () =>
   {
       var metadata = new HealthMetadata
       {
           ServiceName = "My Service",
           Version = "1.2.3",
           Environment = "Production",
           Status = HealthStatus.Healthy
       };
       return Results.Json(metadata);
   });
   ```

   **Note:** The .NET library automatically serializes to camelCase JSON when using the recommended configuration.

### Manual Implementation (Any Language)

Simply create a `GET` endpoint at `/healthz` that returns JSON matching the schema:

**Node.js/Express Example:**
```javascript
app.get('/healthz', (req, res) => {
  res.json({
    serviceName: 'My Node Service',
    version: '1.0.0',
    environment: 'Production',
    status: 'healthy',
    timestamp: new Date().toISOString()
  });
});
```

**Python/Flask Example:**
```python
@app.route('/healthz')
def health():
    return jsonify({
        'serviceName': 'My Python Service',
        'version': '1.0.0',
        'environment': 'Production',
        'status': 'healthy',
        'timestamp': datetime.utcnow().isoformat()
    })
```

## Dynamic Version Information

### CI/CD Integration

You can dynamically populate version information during your CI/CD pipeline:

1. Create an `appsettings.healthmeta.json` file during build:

```json
{
  "serviceName": "My Service",
  "version": "${BUILD_VERSION}",
  "environment": "${DEPLOY_ENVIRONMENT}"
}
```

2. Have your build process replace the tokens:

```bash
# GitHub Actions example
- name: Create health metadata
  run: |
    cat > appsettings.healthmeta.json << EOF
    {
      "serviceName": "My Service",
      "version": "${{ github.sha }}",
      "environment": "${{ github.ref_name }}"
    }
    EOF
```

3. Load this file at startup in your application.

### Git Information

Many CI/CD systems provide environment variables you can use:

| CI/CD System | Version Variable | Branch Variable |
|--------------|------------------|-----------------|
| GitHub Actions | `${{ github.sha }}` | `${{ github.ref_name }}` |
| GitLab CI | `$CI_COMMIT_SHA` | `$CI_COMMIT_BRANCH` |
| Azure DevOps | `$(Build.SourceVersion)` | `$(Build.SourceBranchName)` |
| Jenkins | `$GIT_COMMIT` | `$GIT_BRANCH` |

## Best Practices

1. **Always include required fields**: `serviceName` and `version` are mandatory.

2. **Use meaningful versions**: Whether semantic versioning or git references, make it identifiable.

3. **Update status appropriately**: Don't always return "healthy" - reflect actual service state.

4. **Include useful metadata**: Add information that helps with troubleshooting (database status, cache connectivity, etc.).

5. **Keep responses fast**: Health checks should return quickly (< 1 second).

6. **Make it accessible**: The `/healthz` endpoint should not require authentication for monitoring purposes.

7. **Use UTC timestamps**: Always use UTC time for consistency across regions.

8. **Use camelCase**: All JSON property names should use camelCase for consistency across all services.

## Troubleshooting

### Dashboard Shows "Error" for Service

- Verify the health endpoint URL is correct and accessible
- Check that your endpoint returns valid JSON
- Ensure the response includes at least `serviceName` and `version`
- Check network connectivity between dashboard and service

### Version Not Updating

- Verify your CI/CD pipeline is creating/updating the health metadata file
- Check file permissions if loading from external file
- Restart your service to pick up configuration changes

## FAQ

**Q: Can I use a different endpoint path than `/healthz`?**  
A: Yes! Configure the `healthCheckUrl` in the dashboard configuration to point to any path.

**Q: Do I need to use the NuGet package?**  
A: No, you can manually implement the JSON schema in any language.

**Q: What if my service is unhealthy?**  
A: Your `/healthz` endpoint should still return 200 OK with `status: "unhealthy"` and a descriptive message.

**Q: Can I add custom fields?**  
A: Yes! Use the `additionalMetadata` object for service-specific information.

**Q: How often does the dashboard poll services?**  
A: Configurable via `refreshIntervalSeconds` in `dashboard-config.json` (default: 30 seconds).

**Q: Why camelCase instead of PascalCase?**  
A: camelCase is the standard JSON naming convention used by most APIs (JavaScript, Python, Node.js, etc.). This makes the schema more accessible across different technology stacks. The dashboard is case-insensitive when deserializing, so it can accept both formats for backward compatibility.
