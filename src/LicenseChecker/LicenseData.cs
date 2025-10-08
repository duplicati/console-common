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
/// The license data
/// </summary>
public record LicenseData
{
    /// <summary>
    /// The grace period after license expiration during which the software will still operate
    /// </summary>
    private static readonly TimeSpan GracePeriod = TimeSpan.FromDays(30);

    /// <summary>
    /// When the license is valid from
    /// </summary>
    public required DateTimeOffset ValidFrom { get; init; }
    /// <summary>
    /// When the license is valid to
    /// </summary>
    public required DateTimeOffset ValidTo { get; init; }
    /// <summary>
    /// The features enabled for this license
    /// </summary>
    public required Dictionary<string, string> Features { get; init; } = new();
    /// <summary>
    /// The maximum number of machines allowed for this license
    /// </summary>
    public required int MaxMachines { get; init; }
    /// <summary>
    /// The organization ID associated with the license
    /// </summary>
    public required string OrganizationId { get; init; }
    /// <summary>
    /// The license ID
    /// </summary>
    public required string LicenseId { get; init; }
    /// <summary>
    /// When the license was generated
    /// </summary>
    public required DateTimeOffset GeneratedAt { get; init; }

    /// <summary>
    /// Determines if the license is currently valid
    /// </summary>
    public bool IsValidNow => DateTimeOffset.UtcNow >= ValidFrom && DateTimeOffset.UtcNow <= ValidTo;

    /// <summary>
    /// The expiration date including the grace period
    /// </summary>
    public DateTimeOffset ValidToWithGrace => ValidTo.Add(GracePeriod);

    /// <summary>
    /// Determines if the license is in its grace period
    /// </summary>
    public bool IsInGracePeriod => DateTimeOffset.UtcNow > ValidTo && DateTimeOffset.UtcNow <= ValidToWithGrace;
}