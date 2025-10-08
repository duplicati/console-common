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
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace ConsoleCommon;

/// <summary>
/// Extension methods to add the rate limiting middleware to the HTTP request pipeline.
/// </summary>
public static class RateLimitMiddlewareExtensions
{
    /// <summary>
    /// Configures rate limiting services
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <param name="config">Configuration options. If null, defaults will be used.</param>
    /// <param name="logAction">Optional logging action for rate limit rejection events.</param>
    /// <returns>The updated application builder.</returns>
    public static IHostApplicationBuilder ConfigureRateLimiting(this IHostApplicationBuilder builder, SimpleSecurityOptions config, Action<string>? logAction = null)
    {
        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
                PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        // Get the IP address of the client using x-forwarded-for header
                        context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()
                        ?? context.Connection.RemoteIpAddress?.ToString()
                        ?? Guid.NewGuid().ToString(),
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = config.MaxRequestsPerSecondPerIp,
                            Window = TimeSpan.FromSeconds(1),
                            QueueLimit = 0
                        }))
            );

            options.OnRejected = async (context, token) =>
            {
                // Get the IP address of the client using x-forwarded-for header
                var ipAddress = context.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()
                    ?? context.HttpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";

                logAction?.Invoke($"Rate limit exceeded for {ipAddress} on {context.HttpContext.Request.Path}");
                context.HttpContext.Response.StatusCode = 429;
                await context.HttpContext.Response.WriteAsync("Too many requests", token);
            };
        });

        return builder;
    }
}
