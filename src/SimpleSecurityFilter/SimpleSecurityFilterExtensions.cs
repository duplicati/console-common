using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace ConsoleCommon;

/// <summary>
/// Extension methods to add the SimpleSecurityFilter middleware to the HTTP request pipeline.
/// </summary>
public static class SimpleSecurityFilterExtensions
{
    /// <summary>
    /// Adds the SimpleSecurityFilter middleware to the HTTP request pipeline, if enabled in the configuration.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <param name="config">Configuration options. If null, defaults will be used.</param>
    /// <param name="logAction">Optional logging action for rate limit rejection events.</param>
    /// <returns>The updated application builder.</returns>
    public static IHostApplicationBuilder AddSimpleSecurityFilter(this IHostApplicationBuilder builder, SimpleSecurityOptions? config, Action<string>? logAction = null)
    {
        config ??= SimpleSecurityOptions.Default;
        if (config.RateLimitEnabled)
            builder.ConfigureRateLimiting(config, logAction);

        return builder;
    }

    /// <summary>
    /// Adds the SimpleSecurityFilter middleware to the HTTP request pipeline, if enabled in the configuration.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="config">Configuration options. If null, defaults will be used.</param>
    /// <returns>The updated application builder.</returns>
    public static IApplicationBuilder UseSimpleSecurityFilter(this IApplicationBuilder app, SimpleSecurityOptions? config)
    {
        config ??= SimpleSecurityOptions.Default;
        if (config.RateLimitEnabled)
            app.UseRateLimiter();

        if (config.FilterPatterns)
            app.UseScanningFilter();

        return app;
    }
}
