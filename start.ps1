# Simple Observability - Quick Start Script
# This script helps you quickly start the dashboard and sample services

Write-Host "🚀 Simple Observability - Quick Start" -ForegroundColor Cyan
Write-Host ""

# Check if .NET 10 is installed.
Write-Host "Checking for .NET 10 SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ .NET SDK not found. Please install .NET 10 SDK." -ForegroundColor Red
    Write-Host "   Download from: https://dotnet.microsoft.com/download" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Found .NET SDK version: $dotnetVersion" -ForegroundColor Green
Write-Host ""

# Ask user what to run.
Write-Host "What would you like to run?" -ForegroundColor Cyan
Write-Host "1. Dashboard only"
Write-Host "2. Dashboard + Sample Service"
Write-Host "3. Everything with Docker Compose"
Write-Host ""
$choice = Read-Host "Enter choice (1-3)"

switch ($choice) {
    "1" {
        Write-Host ""
        Write-Host "Starting Dashboard..." -ForegroundColor Cyan
        Set-Location "code\WebApi\WorldDomination.SimpleObservability.WebApi"
        Write-Host "Dashboard will be available at: http://localhost:5000" -ForegroundColor Green
        Write-Host ""
        dotnet run
    }
    "2" {
        Write-Host ""
        Write-Host "Starting Dashboard and Sample Service..." -ForegroundColor Cyan
        Write-Host ""
        Write-Host "📊 Dashboard: http://localhost:5000" -ForegroundColor Green
        Write-Host "🔧 Sample Service: http://localhost:5001" -ForegroundColor Green
        Write-Host ""
        Write-Host "To test changing health status:" -ForegroundColor Yellow
        Write-Host "  curl -X PUT http://localhost:5001/health -H 'Content-Type: application/json' -d '{\"status\":1}'" -ForegroundColor Gray
        Write-Host ""
        Write-Host "Press Ctrl+C to stop all services" -ForegroundColor Yellow
        Write-Host ""
        
        # Start service in background.
        $service1 = Start-Process -FilePath "dotnet" -ArgumentList "run" -WorkingDirectory "code\Samples\SampleService1" -PassThru -WindowStyle Minimized
        
        Start-Sleep -Seconds 3
        
        # Start dashboard in foreground.
        Set-Location "code\WebApi\WorldDomination.SimpleObservability.WebApi"
        
        try {
            dotnet run
        }
        finally {
            # Clean up background processes.
            Write-Host ""
            Write-Host "Stopping sample service..." -ForegroundColor Yellow
            Stop-Process -Id $service1.Id -Force -ErrorAction SilentlyContinue
        }
    }
    "3" {
        # Check for Docker.
        Write-Host ""
        Write-Host "Checking for Docker..." -ForegroundColor Yellow
        $dockerVersion = docker --version 2>$null
        if ($LASTEXITCODE -ne 0) {
            Write-Host "❌ Docker not found. Please install Docker Desktop." -ForegroundColor Red
            Write-Host "   Download from: https://www.docker.com/products/docker-desktop" -ForegroundColor Red
            exit 1
        }
        Write-Host "✅ Found Docker: $dockerVersion" -ForegroundColor Green
        Write-Host ""
        
        Write-Host "Starting with Docker Compose..." -ForegroundColor Cyan
        Write-Host ""
        Write-Host "📊 Dashboard: http://localhost:8080" -ForegroundColor Green
        Write-Host "🔧 Payment API (DEV): http://localhost:5001" -ForegroundColor Green
        Write-Host "🔧 Payment API (UAT): http://localhost:5002" -ForegroundColor Green
        Write-Host "🔧 Payment API (PROD): http://localhost:5003" -ForegroundColor Green
        Write-Host "🔧 User API (DEV): http://localhost:5004" -ForegroundColor Green
        Write-Host ""
        Write-Host "To test changing health status:" -ForegroundColor Yellow
        Write-Host "  curl -X PUT http://localhost:5001/health -H 'Content-Type: application/json' -d '{\"status\":1}'" -ForegroundColor Gray
        Write-Host ""
        Write-Host "Building and starting containers..." -ForegroundColor Yellow
        Write-Host ""
        
        docker-compose up --build
    }
    default {
        Write-Host "Invalid choice. Exiting." -ForegroundColor Red
        exit 1
    }
}
