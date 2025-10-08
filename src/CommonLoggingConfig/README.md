# Common Logging Config

Common logging config across Duplicati console projects.

## Example use

```csharp
using CommonLoggingConfig;

var builder = WebApplication.CreateBuilder(args);
var serilogConfig = builder.Configuration.GetSection("Serilog").Get<SerilogConfig>();
var extras = new LoggingExtras() { IsProd = false };

builder.AddCommonLogging(serilogConfig, extras);

var app = builder.Build();
app.UseCommonLogging();



```
