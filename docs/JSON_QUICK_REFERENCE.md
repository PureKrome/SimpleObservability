# Quick Reference: JSON Property Names

## Standard Schema (camelCase)

All JSON in the Simple Observability ecosystem uses **camelCase** property names.

### Health Metadata Response

```json
{
  "serviceName": "My Service",
  "version": "1.0.0",
  "environment": "Production",
  "status": "healthy",
  "timestamp": "2024-01-15T10:30:00Z",
  "description": "All systems operational",
  "hostName": "my-server-01",
  "uptime": "P1DT2H30M",
  "additionalMetadata": {
    "database": "Connected",
    "cache": "Redis"
  }
}
```

### Health Status Values

- `"healthy"` - Service is operating normally
- `"degraded"` - Service is operational but experiencing issues
- `"unhealthy"` - Service is not functioning correctly

**Note:** Status values are case-insensitive when consumed by the dashboard.

### Dashboard Configuration

```json
{
  "services": [
    {
      "name": "My Service",
      "environment": "PROD",
      "healthCheckUrl": "https://myservice.example.com/healthz",
      "description": "Service description",
      "enabled": true,
      "timeoutSeconds": 10
    }
  ],
  "timeoutSeconds": 5,
  "refreshIntervalSeconds": 30,
  "environmentOrder": ["DEV", "UAT", "PROD"]
}
```

## .NET Configuration

Add to your `Program.cs`:

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});
```

## Testing Your Implementation

Use `curl` to verify your endpoint:

```bash
curl http://localhost:5000/healthz | jq
```

Expected output:
```json
{
  "serviceName": "Your Service",
  "version": "1.0.0",
  "status": "healthy",
  ...
}
```

## Common Mistakes to Avoid

❌ **Don't use PascalCase:**
```json
{
  "ServiceName": "My Service",  // Wrong!
  "Version": "1.0.0"
}
```

✅ **Use camelCase:**
```json
{
  "serviceName": "My Service",  // Correct!
  "version": "1.0.0"
}
```

❌ **Don't use uppercase status:**
```json
{
  "status": "HEALTHY"  // Works but not recommended
}
```

✅ **Use lowercase status:**
```json
{
  "status": "healthy"  // Recommended
}
```

## Language-Specific Examples

### Node.js/Express
```javascript
app.get('/healthz', (req, res) => {
  res.json({
    serviceName: 'My Node Service',
    version: '1.0.0',
    status: 'healthy'
  });
});
```

### Python/Flask
```python
@app.route('/healthz')
def health():
    return jsonify({
        'serviceName': 'My Python Service',
        'version': '1.0.0',
        'status': 'healthy'
    })
```

### Go
```go
type HealthMetadata struct {
    ServiceName string `json:"serviceName"`
    Version     string `json:"version"`
    Status      string `json:"status"`
}
```

### Java/Spring Boot
```java
@JsonNaming(PropertyNamingStrategies.LowerCamelCaseStrategy.class)
public class HealthMetadata {
    private String serviceName;
    private String version;
    private String status;
    // getters and setters
}
```
