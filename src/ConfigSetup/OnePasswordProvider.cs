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
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Serilog;
namespace ConsoleCommon;

public class OnePasswordProvider
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public static void AddEnvironmentVariablesFromVault(WebApplicationBuilder builder)
    {
        var sessionToken = builder.Configuration["1PASSWORD_SESSIONTOKEN"];
        var vault = builder.Configuration["1PASSWORD_VAULT"];

        if (string.IsNullOrWhiteSpace(vault))
        {
            Log.Information("No 1Password vault specified. Skipping vault environment variable loading.");
            return;
        }

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "op",
                Arguments = $"{(string.IsNullOrWhiteSpace(sessionToken) ? "" : $"--session {sessionToken} ")}item get EnvironmentVariables --vault {vault} --reveal --format json",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false
            }
        };

        try
        {
            process.Start();
            var json = process.StandardOutput.ReadToEnd();
            var errors = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                if (errors.Contains("You are not currently signed in", StringComparison.OrdinalIgnoreCase))
                    throw new Exception("[1Password] You are not signed in. Please ensure 1PASSWORD_SESSIONTOKEN is correct or 1Password Desktop App integration is enabled.");

                if (errors.Contains("authorization prompt dismissed", StringComparison.OrdinalIgnoreCase))
                    throw new Exception("[1Password] Authorization prompt was dismissed. Please retry via 1Password Desktop App.");

                throw new Exception($"[1Password] {errors.Trim()}");
            }

            var item = JsonSerializer.Deserialize<OnePasswordItem>(json, SerializerOptions);

            var secrets = item?.Fields?
                .Where(f => !string.IsNullOrWhiteSpace(f.Label) && !string.IsNullOrWhiteSpace(f.Value))
                .ToDictionary(f => f.Label!, f => f.Value!);

            if (secrets is null || secrets.Count == 0)
            {
                Log.Warning($"[1Password] No secrets found in vault '{vault}'.");
                return;
            }

            Log.Information($"[1Password] Loaded {secrets.Count} secrets from vault '{vault}'.");

            foreach (var (key, value) in secrets)
            {
                if (string.IsNullOrEmpty(value)) continue;

                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
                {
                    Environment.SetEnvironmentVariable(key, value);
                    Log.Debug($"[1Password] Set environment variable '{key}' from vault.");
                }
                else
                {
                    Log.Debug($"[1Password] Skipped existing environment variable '{key}'.");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"[1Password] Failed to retrieve secrets from vault '{vault}'.", ex);
            throw new Exception($"[1Password] Failed to retrieve secrets from vault '{vault}': {ex.Message}", ex);
        }
    }

    public class OnePasswordItem(List<OnePasswordField>? fields)
    {
        public List<OnePasswordField>? Fields { get; init; } = fields;
    }

    public class OnePasswordField(string? value, string? label)
    {
        public string? Label { get; } = label;
        public string? Value { get; } = value;
    }
}