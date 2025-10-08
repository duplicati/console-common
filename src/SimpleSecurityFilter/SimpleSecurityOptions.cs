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
namespace ConsoleCommon;

/// <summary>
/// Security middleware settings
/// </summary>
public sealed record SimpleSecurityOptions
{
    /// <summary>
    /// Determine if the middleware should filter preset attack patterns.
    /// </summary>
    public bool FilterPatterns { get; init; }

    /// <summary>
    /// Determine if the middleware should rate limit requests.
    /// </summary>
    public bool RateLimitEnabled { get; init; }

    /// <summary>
    /// Number of requests per second allowed per IP.
    /// </summary>
    public int MaxRequestsPerSecondPerIp { get; init; }

    /// <summary>
    /// Provides a default configuration, that enables pattern filtering but disables rate limiting.
    /// </summary>
    public static SimpleSecurityOptions Default => new()
    {
        FilterPatterns = true,
        RateLimitEnabled = false,
        MaxRequestsPerSecondPerIp = 10
    };
}