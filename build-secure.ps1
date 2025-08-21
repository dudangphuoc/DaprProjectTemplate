# PowerShell script for DX Dapr Template Build & Publish
param(
    [Parameter(Mandatory=$false)]
    [string]$ApiKey,
    
    [Parameter(Mandatory=$false)]
    [string]$Source = "https://baget.hexbox.vn/v3/index.json",
    
    [Parameter(Mandatory=$false)]
    [ValidateSet("Clean", "Compile", "Pack", "Publish")]
    [string]$Target = "Pack",
    
    [Parameter(Mandatory=$false)]
    [switch]$SetupEnvironment
)

Write-Host "?? DX Dapr Template Build Script" -ForegroundColor Green

if ($SetupEnvironment) {
    Write-Host "?? Setting up build environment..." -ForegroundColor Yellow
    
    if (-not (Test-Path "build\.env")) {
        Copy-Item "build\.env.template" "build\.env"
        Write-Host "? Created build\.env from template" -ForegroundColor Green
        Write-Host "?? Please edit build\.env and add your API key" -ForegroundColor Cyan
    } else {
        Write-Host "? build\.env already exists" -ForegroundColor Green
    }
    
    Write-Host "?? Environment setup complete!" -ForegroundColor Green
    return
}

# Load environment variables from .env file if exists
if (Test-Path "build\.env") {
    Write-Host "?? Loading environment variables from build\.env..." -ForegroundColor Cyan
    Get-Content "build\.env" | ForEach-Object {
        if ($_ -match '^([^#][^=]+)=(.+)$') {
            [Environment]::SetEnvironmentVariable($matches[1], $matches[2], "Process")
        }
    }
}

# Set API key if provided
if ($ApiKey) {
    $env:NUGET_API_KEY = $ApiKey
}

# Set source if provided
if ($Source) {
    $env:NUGET_SOURCE = $Source
}

Write-Host "?? Build Target: $Target" -ForegroundColor Cyan

switch ($Target) {
    "Clean" {
        Write-Host "?? Cleaning build artifacts..." -ForegroundColor Yellow
        dotnet run --project build -- Clean
    }
    "Compile" {
        Write-Host "?? Compiling solution..." -ForegroundColor Yellow
        dotnet run --project build -- Compile
    }
    "Pack" {
        Write-Host "?? Packing NuGet packages..." -ForegroundColor Yellow
        dotnet run --project build -- Pack
    }
    "Publish" {
        Write-Host "?? Publishing to NuGet..." -ForegroundColor Yellow
        
        if (-not $env:NUGET_API_KEY) {
            Write-Host "? Error: NUGET_API_KEY environment variable is required for publishing!" -ForegroundColor Red
            Write-Host "?? Set it using:" -ForegroundColor Cyan
            Write-Host "   `$env:NUGET_API_KEY = 'your-api-key'" -ForegroundColor White
            Write-Host "   or use: .\build-secure.ps1 -ApiKey 'your-api-key' -Target Publish" -ForegroundColor White
            exit 1
        }
        
        $maskedKey = $env:NUGET_API_KEY.Substring(0, [Math]::Min(5, $env:NUGET_API_KEY.Length)) + "***"
        Write-Host "?? Using API Key: $maskedKey" -ForegroundColor Green
        Write-Host "?? Publishing to: $env:NUGET_SOURCE" -ForegroundColor Green
        
        dotnet run --project build -- Publish
    }
}

if ($LASTEXITCODE -eq 0) {
    Write-Host "? Build completed successfully!" -ForegroundColor Green
} else {
    Write-Host "? Build failed with exit code: $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}