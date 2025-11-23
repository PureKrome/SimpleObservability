# Simple Observability - Examples

This document provides various examples of implementing and using Simple Observability.

## Table of Contents

1. [Basic Health Check Implementation](#basic-health-check-implementation)
2. [Advanced Health Check with Custom Metadata](#advanced-health-check-with-custom-metadata)
3. [Loading Health Metadata from Configuration](#loading-health-metadata-from-configuration)
4. [Loading Health Metadata from Separate File](#loading-health-metadata-from-separate-file)
5. [Dynamic Health Status Based on Dependencies](#dynamic-health-status-based-on-dependencies)
6. [CI/CD Integration Examples](#cicd-integration-examples)
7. [Dashboard Configuration Examples](#dashboard-configuration-examples)

---

## Basic Health Check Implementation

### .NET Minimal API

```csharp
using WorldDomination.SimpleObservability;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/healthz", () =>
{
    return Results.Json(new HealthMetadata
    {
        ServiceName = "My API",
        Version = "1.0.0"
    });
});

app.Run();
```

### .NET with Controllers

```csharp
using Microsoft.AspNetCore.Mvc;
using WorldDomination.SimpleObservability;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet("/healthz")]
    public ActionResult<HealthMetadata> GetHealth()
    {
        var metadata = new HealthMetadata
        {
            ServiceName = "My API",
            Version = "1.0.0",
            Environment = "Production",
            Status = HealthStatus.Healthy
        };
        
        return Ok(metadata);
    }
}
```

---

## Advanced Health Check with Custom Metadata

```csharp
using WorldDomination.SimpleObservability;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Track startup time.
var startTime = DateTimeOffset.UtcNow;
var processStart = Process.GetCurrentProcess().StartTime;

var app = builder.Build();

app.MapGet("/healthz", () =>
{
    var currentProcess = Process.GetCurrentProcess();
    var uptime = DateTimeOffset.UtcNow - startTime;
    
    var metadata = new HealthMetadata
    {
        ServiceName = "Advanced API",
        Version = "2.3.1",
        Environment = "Production",
        Status = HealthStatus.Healthy,
        Description = "All systems operational",
        HostName = Environment.MachineName,
        Uptime = uptime,
        AdditionalMetadata = new Dictionary<string, string>
        {
            ["Runtime"] = $".NET {Environment.Version}",
            ["WorkingSet"] = $"{currentProcess.WorkingSet64 / 1024 / 1024} MB",
            ["ThreadCount"] = currentProcess.Threads.Count.ToString(),
            ["OS"] = Environment.OSVersion.ToString(),
            ["ProcessorCount"] = Environment.ProcessorCount.ToString(),
            ["Region"] = "us-west-2",
            ["Instance"] = Environment.GetEnvironmentVariable("INSTANCE_ID") ?? "local"
        }
    };
    
    return Results.Json(metadata);
});

app.Run();
```

---

## Loading Health Metadata from Configuration

### appsettings.json

```json
{
  "HealthMetadata": {
    "ServiceName": "Configuration-Based API",
    "Version": "1.5.0",
    "Environment": "Production",
    "Status": "Healthy",
    "Description": "Production deployment"
  }
}
```

### Program.cs

```csharp
using WorldDomination.SimpleObservability;

var builder = WebApplication.CreateBuilder(args);

// Bind configuration to a POCO.
var healthConfig = new HealthMetadataConfig();
builder.Configuration.GetSection("HealthMetadata").Bind(healthConfig);

var app = builder.Build();

app.MapGet("/healthz", () =>
{
    return Results.Json(new HealthMetadata
    {
        ServiceName = healthConfig.ServiceName ?? "Unknown",
        Version = healthConfig.Version ?? "0.0.0",
        Environment = healthConfig.Environment,
        Status = Enum.Parse<HealthStatus>(healthConfig.Status ?? "Healthy"),
        Description = healthConfig.Description,
        Timestamp = DateTimeOffset.UtcNow
    });
});

app.Run();

public class HealthMetadataConfig
{
    public string? ServiceName { get; set; }
    public string? Version { get; set; }
    public string? Environment { get; set; }
    public string? Status { get; set; }
    public string? Description { get; set; }
}
```

---

## Loading Health Metadata from Separate File

### appsettings.healthmeta.json (created by CI/CD)

```json
{
  "serviceName": "My Service",
  "version": "abc123def456",
  "environment": "Production",
  "status": 0,
  "description": "Deployed from main branch"
}
```

### Program.cs

```csharp
using WorldDomination.SimpleObservability;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Load health metadata from file if it exists.
var healthMetadataFile = "appsettings.healthmeta.json";
HealthMetadata healthMetadata;

if (File.Exists(healthMetadataFile))
{
    var json = await File.ReadAllTextAsync(healthMetadataFile);
    healthMetadata = JsonSerializer.Deserialize<HealthMetadata>(json,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
        ?? throw new InvalidOperationException("Failed to load health metadata");
}
else
{
    // Fallback to default.
    healthMetadata = new HealthMetadata
    {
        ServiceName = "My Service",
        Version = "0.0.0-dev",
        Environment = "Development"
    };
}

var startTime = DateTimeOffset.UtcNow;

var app = builder.Build();

app.MapGet("/healthz", () =>
{
    // Return metadata with updated timestamp and uptime.
    return Results.Json(healthMetadata with
    {
        Timestamp = DateTimeOffset.UtcNow,
        Uptime = DateTimeOffset.UtcNow - startTime
    });
});

app.Run();
```

---

## Dynamic Health Status Based on Dependencies

```csharp
using WorldDomination.SimpleObservability;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IDatabaseConnection, DatabaseConnection>();
builder.Services.AddSingleton<ICacheService, CacheService>();

var app = builder.Build();

app.MapGet("/healthz", async (
    IDatabaseConnection database,
    ICacheService cache,
    IHttpClientFactory httpClientFactory) =>
{
    var checks = new List<string>();
    var status = HealthStatus.Healthy;
    
    // Check database connection.
    try
    {
        await database.CheckConnectionAsync();
        checks.Add("Database: ✅");
    }
    catch
    {
        checks.Add("Database: ❌");
        status = HealthStatus.Unhealthy;
    }
    
    // Check cache connection.
    try
    {
        await cache.PingAsync();
        checks.Add("Cache: ✅");
    }
    catch
    {
        checks.Add("Cache: ⚠️");
        status = status == HealthStatus.Unhealthy 
            ? HealthStatus.Unhealthy 
            : HealthStatus.Degraded;
    }
    
    var metadata = new HealthMetadata
    {
        ServiceName = "Dependency-Aware API",
        Version = "1.0.0",
        Environment = "Production",
        Status = status,
        Description = string.Join(", ", checks),
        AdditionalMetadata = new Dictionary<string, string>
        {
            ["DatabaseStatus"] = checks[0],
            ["CacheStatus"] = checks[1]
        }
    };
    
    return Results.Json(metadata);
});

app.Run();
```

---

## CI/CD Integration Examples

### GitHub Actions - Build and Deploy

```yaml
name: Build and Deploy

on:
  push:
    branches: [main, develop]

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      
      - name: Create health metadata
        run: |
          cat > appsettings.healthmeta.json << EOF
          {
            "serviceName": "My Service",
            "version": "${{ github.sha }}",
            "environment": "${{ github.ref_name }}",
            "status": 0,
            "description": "Deployed from GitHub Actions",
            "additionalMetadata": {
              "branch": "${{ github.ref_name }}",
              "commit": "${{ github.sha }}",
              "actor": "${{ github.actor }}",
              "runNumber": "${{ github.run_number }}"
            }
          }
          EOF
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build --no-restore -c Release
      
      - name: Publish
        run: dotnet publish -c Release -o ./publish
      
      - name: Copy health metadata to publish folder
        run: cp appsettings.healthmeta.json ./publish/
      
      - name: Deploy
        run: |
          # Your deployment commands here
          echo "Deploying to ${{ github.ref_name }} environment"
```

### GitLab CI - Multi-Environment

```yaml
stages:
  - build
  - deploy

variables:
  SERVICE_NAME: "My Service"

.create_health_metadata:
  script:
    - |
      cat > appsettings.healthmeta.json << EOF
      {
        "serviceName": "${SERVICE_NAME}",
        "version": "${CI_COMMIT_SHA}",
        "environment": "${CI_ENVIRONMENT_NAME}",
        "status": 0,
        "description": "Deployed from GitLab CI",
        "additionalMetadata": {
          "branch": "${CI_COMMIT_BRANCH}",
          "commit": "${CI_COMMIT_SHORT_SHA}",
          "pipelineId": "${CI_PIPELINE_ID}",
          "jobId": "${CI_JOB_ID}"
        }
      }
      EOF

build:
  stage: build
  script:
    - dotnet restore
    - dotnet build -c Release
    - dotnet publish -c Release -o ./publish
  artifacts:
    paths:
      - ./publish

deploy:dev:
  stage: deploy
  extends: .create_health_metadata
  environment:
    name: development
  script:
    - cp appsettings.healthmeta.json ./publish/
    - # Deploy to dev environment
  only:
    - develop

deploy:prod:
  stage: deploy
  extends: .create_health_metadata
  environment:
    name: production
  script:
    - cp appsettings.healthmeta.json ./publish/
    - # Deploy to prod environment
  only:
    - main
```

### Azure DevOps Pipeline

```yaml
trigger:
  branches:
    include:
      - main
      - develop

pool:
  vmImage: 'ubuntu-latest'

variables:
  serviceName: 'My Service'

stages:
- stage: Build
  jobs:
  - job: BuildJob
    steps:
    - task: UseDotNet@2
      inputs:
        version: '10.0.x'
    
    - script: |
        cat > appsettings.healthmeta.json << EOF
        {
          "serviceName": "$(serviceName)",
          "version": "$(Build.SourceVersion)",
          "environment": "$(Build.SourceBranchName)",
          "status": 0,
          "description": "Built by Azure DevOps",
          "additionalMetadata": {
            "buildId": "$(Build.BuildId)",
            "buildNumber": "$(Build.BuildNumber)",
            "repository": "$(Build.Repository.Name)"
          }
        }
        EOF
      displayName: 'Create health metadata'
    
    - task: DotNetCoreCLI@2
      inputs:
        command: 'restore'
    
    - task: DotNetCoreCLI@2
      inputs:
        command: 'build'
        arguments: '-c Release'
    
    - task: DotNetCoreCLI@2
      inputs:
        command: 'publish'
        publishWebProjects: true
        arguments: '-c Release -o $(Build.ArtifactStagingDirectory)'
    
    - script: |
        cp appsettings.healthmeta.json $(Build.ArtifactStagingDirectory)/
      displayName: 'Copy health metadata'
    
    - task: PublishBuildArtifacts@1
      inputs:
        PathtoPublish: '$(Build.ArtifactStagingDirectory)'
        ArtifactName: 'drop'
```

---

## Dashboard Configuration Examples

### Simple Configuration

```json
{
  "environments": ["DEV", "PROD"],
  "services": [
    {
      "name": "API",
      "environment": "DEV",
      "healthCheckUrl": "https://api-dev.example.com/healthz",
      "enabled": true
    },
    {
      "name": "API",
      "environment": "PROD",
      "healthCheckUrl": "https://api.example.com/healthz",
      "enabled": true
    }
  ],
  "refreshIntervalSeconds": 30,
  "timeoutSeconds": 5
}
```

### Complex Multi-Service Configuration

```json
{
  "environments": ["DEV", "QA", "UAT", "STAGING", "PROD"],
  "services": [
    {
      "name": "Auth Service",
      "environment": "PROD",
      "healthCheckUrl": "https://auth.example.com/healthz",
      "description": "Authentication and authorization",
      "enabled": true
    },
    {
      "name": "User API",
      "environment": "PROD",
      "healthCheckUrl": "https://users.example.com/healthz",
      "description": "User management",
      "enabled": true
    },
    {
      "name": "Payment API",
      "environment": "PROD",
      "healthCheckUrl": "https://payments.example.com/healthz",
      "description": "Payment processing",
      "enabled": true
    },
    {
      "name": "Notification Service",
      "environment": "PROD",
      "healthCheckUrl": "https://notifications.example.com/healthz",
      "description": "Email and SMS notifications",
      "enabled": true
    },
    {
      "name": "Auth Service",
      "environment": "UAT",
      "healthCheckUrl": "https://auth-uat.example.com/healthz",
      "enabled": true
    },
    {
      "name": "User API",
      "environment": "UAT",
      "healthCheckUrl": "https://users-uat.example.com/healthz",
      "enabled": true
    }
  ],
  "refreshIntervalSeconds": 15,
  "timeoutSeconds": 10
}
```

### Kubernetes Internal Services

```json
{
  "environments": ["Production"],
  "services": [
    {
      "name": "Auth Service",
      "environment": "Production",
      "healthCheckUrl": "http://auth-service.default.svc.cluster.local/healthz",
      "description": "Internal K8s service",
      "enabled": true
    },
    {
      "name": "User Service",
      "environment": "Production",
      "healthCheckUrl": "http://user-service.default.svc.cluster.local/healthz",
      "enabled": true
    }
  ],
  "refreshIntervalSeconds": 30,
  "timeoutSeconds": 5
}
```

---

## Testing Examples

### Unit Test for Health Endpoint

```csharp
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using WorldDomination.SimpleObservability;

public class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsValidMetadata()
    {
        // Arrange.
        var client = _factory.CreateClient();

        // Act.
        var response = await client.GetAsync("/healthz");

        // Assert.
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(content);
        
        Assert.NotNull(metadata);
        Assert.NotNull(metadata.ServiceName);
        Assert.NotNull(metadata.Version);
    }
}
```

This completes the comprehensive examples document!
