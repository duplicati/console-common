# Config Setup

Configuration setup for development environments in Duplicati console applications.

This component provides utilities to configure ASP.NET Core applications for development, including loading environment variables from local JSON files and integrating with 1Password for secure secret management.

## Features

- Automatic loading of local environment variables from `local.environmentvariables.json` in development mode
- Integration with 1Password CLI to load secrets from vaults
- Environment variable precedence handling

## Requirements

- 1Password CLI (`op`) installed and authenticated for vault access (optional, only if using 1Password integration)
- `local.environmentvariables.json` file in the project root (optional)

## Example Use

```csharp
using ConsoleCommon;

var builder = WebApplication.CreateBuilder(args);

// Configure for development (loads local env vars and 1Password secrets if available)
builder = builder.ConfigureForDevelopment();

var app = builder.Build();
```

## Configuration

Set the following environment variables or configuration values:

- `1PASSWORD_SESSIONTOKEN`: Session token for 1Password CLI (optional)
- `1PASSWORD_VAULT`: Name of the 1Password vault to load secrets from (optional)

If `1PASSWORD_VAULT` is not set, 1Password integration is skipped.
If `1PASSWORD_SESSIONTOKEN` is set, then you will not be prompted to unlock the vault.

The vault must have an entry with the name `EnvironmentVariables` and inside that, a list of password or text items, where the name is will be the name of the environment variable, and the password or text will be the value of the environment variable.
