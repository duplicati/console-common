# Simple Security Filter

This package contains a basic security filter for use in publicly available endpoints using ASP.NET Core.

The filters use a basic pattern matching to prevent calls to known vulnerability scanning endpoints and return a custom error code.

Besides the filters, this package also has a rate limiter config that leverages the .NET ratelimiter, but with a simpler API.

## Example usage:

```csharp
using SimpleSecurityFilter;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration.GetSection("SimpleSecurity").Get<SimpleSecurityOptions>();

builder.AddSimpleSecurityFilter(config);

var app = builder.Build();
app.UseSimpleSecurityFilter(config);
```
