# Console Common

Common functions for Duplicati console applications.

This library provides shared functionality used across Duplicati's console-based applications, including license validation, logging configuration, and security filtering.

## Components

### License Checker

Provides functionality to validate licenses for the ConsoleCommon library. Supports obtaining licenses from remote servers, local files, or base64-encoded data, with RSA-SHA256 signature verification.

Key features:

- License validation with cryptographic signatures
- Support for multiple license sources (HTTP, file system, base64)
- Automatic expiration checking with grace period
- Feature-based license validation
- Custom exception handling

### Common Logging Config

Shared logging configuration across Duplicati console projects using Serilog.

Features:

- Standardized logging setup for console applications
- Integration with ASP.NET Core hosting
- Environment-specific configuration options

### Simple Security Filter

Basic security filter for publicly available endpoints in ASP.NET Core applications.

Features:

- Pattern matching to prevent vulnerability scanning
- Rate limiting with simplified API
- Custom error responses for blocked requests

## Installation

```bash
dotnet add package DuplicatiConsoleCommon
```

## Usage

### License Validation

```csharp
using ConsoleCommon;

// Obtain a license from the server
var license = await LicenseChecker.ObtainLicenseAsync("your-license-key", CancellationToken.None);

// Check for required features
license.EnsureFeatures(ConsoleLicenseFeatures.GatewayMachineServer);

// Use the license data
Console.WriteLine($"License valid until: {license.ValidTo}");
```

### Logging Configuration

```csharp
using CommonLoggingConfig;

var builder = WebApplication.CreateBuilder(args);
var serilogConfig = builder.Configuration.GetSection("Serilog").Get<SerilogConfig>();
var extras = new LoggingExtras() { IsProd = false };

builder.AddCommonLogging(serilogConfig, extras);

var app = builder.Build();
app.UseCommonLogging();
```

### Security Filtering

```csharp
using SimpleSecurityFilter;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration.GetSection("SimpleSecurity").Get<SimpleSecurityOptions>();

builder.AddSimpleSecurityFilter(config);

var app = builder.Build();
app.UseSimpleSecurityFilter(config);
```

## Requirements

- .NET 8.0 or later
- ASP.NET Core for web-related components

## License

MIT License - see LICENSE file for details.
