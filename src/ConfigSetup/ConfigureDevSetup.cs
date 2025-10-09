
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ConsoleCommon;

/// <summary>
/// Configuration setup for development environment
/// </summary>
public static class ConfigureDevSetup
{
    /// <summary>
    /// Configures the application for development environment, loading local environment variables if present.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    public static WebApplicationBuilder ConfigureForDevelopment(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            // Load into environment variables
            var localEnvironmentVariables = new ConfigurationBuilder()
                   .AddJsonFile("local.environmentvariables.json", optional: true, reloadOnChange: false)
                   .Build().AsEnumerable().ToList();

            foreach (var (key, value) in localEnvironmentVariables)
                Environment.SetEnvironmentVariable(key, value);
        }

        builder.Configuration.AddEnvironmentVariables();
        return builder;
    }
}