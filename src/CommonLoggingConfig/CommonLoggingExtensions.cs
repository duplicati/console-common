// Copyright (c) 2025 Duplicati Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ConsoleCommon;

/// <summary>
/// Extension methods for adding common logging configuration
/// </summary>
public static class CommonLoggingExtensions
{
    /// <summary>
    /// Adds common logging configuration to the WebApplicationBuilder
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder</param>
    /// <param name="extras">The logging extras</param>
    /// <param name="serilogConfig">The Serilog configuration</param>
    /// <param name="configureLogger">An optional action to configure the logger</param>
    /// <returns>The WebApplicationBuilder</returns>
    public static WebApplicationBuilder AddCommonLogging(this WebApplicationBuilder builder, SerilogConfig? serilogConfig, LoggingExtras? extras, Action<LoggerConfiguration>? configureLogger = null)
    {
        builder.Host.UseSerilog((context, services, config) =>
        {
            config.Enrich.FromLogContext()
                .Enrich.FromLogContext()
                .Enrich.WithClientIp()
                .Enrich.WithCorrelationId(headerName: "X-Request-Id")
                .Enrich.WithRequestHeader("X-Forwarded-For")
                .Enrich.WithRequestHeader("X-Forwarded-Proto")
                .Enrich.WithRequestHeader("User-Agent")
                .WriteTo.Console();

            if (extras != null)
                foreach (var p in extras.GetType().GetProperties())
                {
                    var v = p.GetValue(extras)?.ToString();
                    if (!string.IsNullOrWhiteSpace(v))
                        config.Enrich.WithProperty(p.Name, v);
                }

            if (!string.IsNullOrWhiteSpace(serilogConfig?.SourceToken))
            {
                if (string.IsNullOrWhiteSpace(serilogConfig.Endpoint))
                {
                    config.WriteTo.BetterStack(
                        sourceToken: serilogConfig.SourceToken
                    );
                }
                else
                {
                    config.WriteTo.BetterStack(
                        sourceToken: serilogConfig.SourceToken,
                        betterStackEndpoint: serilogConfig.Endpoint
                    );
                }
            }

            configureLogger?.Invoke(config);
        });
        builder.Services.AddHttpContextAccessor();

        return builder;
    }

    /// <summary>
    /// Uses common logging configuration for the application
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder</returns>
    public static IApplicationBuilder UseCommonLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                var proto = httpContext.Request.Headers["X-Forwarded-Proto"].FirstOrDefault() ?? httpContext.Request.Scheme;
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("HttpRequestType", httpContext.Request.Method);
                diagnosticContext.Set("HttpRequestUrl", $"{proto}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}");
                diagnosticContext.Set("HttpRequestId", httpContext.TraceIdentifier);
            };
        });

        return app;
    }
}
