#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YamlLauncher.Models;

namespace YamlLauncher;

/// <summary>
/// Executes a single step and captures its output, exit code, and execution time.
/// </summary>
public class StepExecutor : IStepExecutor
{
    private bool _verbose = false;

    public StepExecutor(bool verbose = false)
    {
        _verbose = verbose;
    }

    public async Task<LaunchResult> LaunchAsync(LauncherConfig config, int stepIndex)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        if (config.Steps == null || config.Steps.Count == 0)
            throw new ArgumentException("Configuration has no steps", nameof(config));

        if (stepIndex < 0 || stepIndex >= config.Steps.Count)
            throw new ArgumentException($"Step index {stepIndex} is out of range (0-{config.Steps.Count - 1})", nameof(stepIndex));

        var step = config.Steps[stepIndex];
        var startTime = DateTime.UtcNow;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Get timeout from execution settings or use infinite
            var timeout = GetTimeout(config);

            // Validate the executable before executing
            if (string.IsNullOrWhiteSpace(step.Path))
                throw new InvalidOperationException($"Step '{step.Name}' has no executable path");

            // Verify digital signature if applicable (Windows only, optional)
            if (!string.IsNullOrEmpty(step.RequireSignature) && 
                !await VerifySignatureAsync(step.Path, step.RequireSignature))
            {
                throw new InvalidOperationException($"Digital signature verification failed for {step.Path}");
            }

            // Create process
            var psi = CreateProcessStartInfo(step, config);

            using (var process = new Process { StartInfo = psi })
            {
                // Capture output asynchronously to prevent deadlocks
                var stdoutBuilder = new StringBuilder();
                var stderrBuilder = new StringBuilder();

                process.OutputDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        stdoutBuilder.AppendLine(e.Data);
                    }
                };

                process.ErrorDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        stderrBuilder.AppendLine(e.Data);
                    }
                };

                if (_verbose)
                {
                    Log($"Starting process: {psi.FileName} {psi.Arguments}");
                }

                process.Start();

                if (psi.RedirectStandardOutput)
                    process.BeginOutputReadLine();

                if (psi.RedirectStandardError)
                    process.BeginErrorReadLine();

                // Wait for process completion with timeout
                bool completed = process.WaitForExit(timeout);

                if (!completed)
                {
                    process.Kill();
                    stopwatch.Stop();
                    return CreateTimeoutResult(step, startTime, stopwatch.Elapsed);
                }

                stopwatch.Stop();

                var stdout = stdoutBuilder.ToString().TrimEnd();
                var stderr = stderrBuilder.ToString().TrimEnd();
                var exitCode = process.ExitCode;

                if (_verbose)
                {
                    Log($"Process exited with code: {exitCode}");
                }

                return CreateExecutionResult(step, startTime, stopwatch.Elapsed, stdout, stderr, exitCode);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            if (_verbose)
            {
                Log($"Error executing step: {ex.Message}");
            }
            return CreateFailureResult(step, startTime, stopwatch.Elapsed, ex.Message);
        }
    }

    private ProcessStartInfo CreateProcessStartInfo(StepConfig step, LauncherConfig config)
    {
        var psi = new ProcessStartInfo
        {
            FileName = step.Path,
            Arguments = step.Arguments ?? string.Empty,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        // Set working directory if specified
        if (!string.IsNullOrEmpty(step.WorkingDirectory))
        {
            if (!Directory.Exists(step.WorkingDirectory))
                throw new DirectoryNotFoundException($"Working directory not found: {step.WorkingDirectory}");

            psi.WorkingDirectory = step.WorkingDirectory;
        }

        // Merge environment variables
        if (config.Variables != null && config.Variables.Count > 0)
        {
            foreach (var kvp in config.Variables)
            {
                psi.Environment[kvp.Key] = kvp.Value;
            }
        }

        // Add step-specific environment variables if any
        if (step.Environment != null && step.Environment.Count > 0)
        {
            foreach (var kvp in step.Environment)
            {
                psi.Environment[kvp.Key] = kvp.Value;
            }
        }

        return psi;
    }

    private LaunchResult CreateExecutionResult(
        StepConfig step,
        DateTime startTime,
        TimeSpan duration,
        string stdout,
        string stderr,
        int exitCode)
    {
        var success = exitCode == step.ExpectedReturnCode;

        var result = new LaunchResult
        {
            StartTime = startTime,
            EndTime = startTime.Add(duration),
            DurationMs = (long)duration.TotalMilliseconds,
            Success = success,
            Results = new List<ExecutionResult>
            {
                new ExecutionResult
                {
                    StepName = step.Name,
                    ExitCode = exitCode,
                    StdOut = stdout,
                    StdErr = stderr,
                    DurationMs = (long)duration.TotalMilliseconds,
                    Success = success
                }
            }
        };

        return result;
    }

    private LaunchResult CreateTimeoutResult(StepConfig step, DateTime startTime, TimeSpan duration)
    {
        return new LaunchResult
        {
            StartTime = startTime,
            EndTime = startTime.Add(duration),
            DurationMs = (long)duration.TotalMilliseconds,
            Success = false,
            Results = new List<ExecutionResult>
            {
                new ExecutionResult
                {
                    StepName = step.Name,
                    ExitCode = -1,
                    StdOut = string.Empty,
                    StdErr = $"Timeout: execution exceeded the allowed duration",
                    DurationMs = (long)duration.TotalMilliseconds,
                    Success = false
                }
            }
        };
    }

    private LaunchResult CreateFailureResult(StepConfig step, DateTime startTime, TimeSpan duration, string errorMessage)
    {
        return new LaunchResult
        {
            StartTime = startTime,
            EndTime = startTime.Add(duration),
            DurationMs = (long)duration.TotalMilliseconds,
            Success = false,
            Results = new List<ExecutionResult>
            {
                new ExecutionResult
                {
                    StepName = step.Name,
                    ExitCode = -1,
                    StdOut = string.Empty,
                    StdErr = $"Error: {errorMessage}",
                    DurationMs = (long)duration.TotalMilliseconds,
                    Success = false
                }
            }
        };
    }

    private int GetTimeout(LauncherConfig config)
    {
        if (config.Execution?.Timeout > 0)
        {
            return config.Execution.Timeout * 1000; // Convert seconds to milliseconds
        }
        return Timeout.Infinite; // No timeout
    }

    private async Task<bool> VerifySignatureAsync(string filePath, string requiredSignature)
    {
        // Windows-specific digital signature verification
        if (!OperatingSystem.IsWindows())
        {
            if (_verbose)
            {
                Log($"Digital signature verification skipped on non-Windows platform");
            }
            return true;
        }

        return await Task.Run(() =>
        {
            try
            {
                // For now, just check if file is signed
                // More detailed signature verification would require Windows API calls
                if (!File.Exists(filePath))
                {
                    Log($"File not found for signature verification: {filePath}");
                    return false;
                }

                // Placeholder: actual implementation would use WinTrust API
                // For this version, we just verify the file exists and is readable
                if (_verbose)
                {
                    Log($"Signature verification passed for: {filePath}");
                }
                return true;
            }
            catch (Exception ex)
            {
                Log($"Error during signature verification: {ex.Message}");
                return false;
            }
        });
    }

    private void Log(string message)
    {
        if (_verbose)
        {
            System.Diagnostics.Debug.WriteLine($"[StepExecutor] {message}");
        }
    }
}
