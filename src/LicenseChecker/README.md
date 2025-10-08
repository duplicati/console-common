# License Checker

This package provides functionality to validate licenses for the ConsoleCommon library. It supports obtaining licenses from a remote server, local files, or base64-encoded data, and verifies them using cryptographic signatures.

Key features include:

- License validation with RSA-SHA256 signature verification
- Support for license retrieval from HTTP server, file system, or base64 strings
- Automatic expiration checking with a 30-day grace period
- Feature-based license validation
- Custom exception handling for invalid licenses

## Example usage:

```csharp
using ConsoleCommon;

// Obtain a license from the server
var license = await LicenseChecker.ObtainLicenseAsync("your-license-key", CancellationToken.None);

// Check for required features
license.EnsureFeatures(ConsoleLicenseFeatures.GatewayMachineServer);

// Use the license data
Console.WriteLine($"License valid until: {license.ValidTo}");
```
