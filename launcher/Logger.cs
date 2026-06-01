#nullable enable

using System;

namespace YamlLauncher.Logging;

/// <summary>
/// Standard implementation of ILogger for ntools-launcher.
/// Outputs messages with appropriate log level prefixes ([INFO], [WARN], [ERROR], [VERBOSE]).
/// </summary>
public class Logger : ILogger
{
    private readonly bool _verbose;

    /// <summary>
    /// Creates a new Logger instance.
    /// </summary>
    /// <param name="verbose">Whether to enable verbose/debug logging.</param>
    public Logger(bool verbose = false)
    {
        _verbose = verbose;
    }

    /// <summary>
    /// Gets a value indicating whether verbose logging is enabled.
    /// </summary>
    public bool IsVerbose => _verbose;

    /// <summary>
    /// Logs an informational message to stdout.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void LogInfo(string message)
    {
        Console.WriteLine($"[LAUNCHER] [INFO] {message}");
    }

    /// <summary>
    /// Logs a warning message to stderr.
    /// </summary>
    /// <param name="message">The warning message to log.</param>
    public void LogWarning(string message)
    {
        Console.Error.WriteLine($"[LAUNCHER] [WARN] {message}");
    }

    /// <summary>
    /// Logs an error message to stderr.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    public void LogError(string message)
    {
        Console.Error.WriteLine($"[LAUNCHER] [ERROR] {message}");
    }

    /// <summary>
    /// Logs a verbose/debug message to stdout if verbose mode is enabled.
    /// </summary>
    /// <param name="message">The verbose message to log.</param>
    public void LogVerbose(string message)
    {
        if (_verbose)
        {
            Console.WriteLine($"[LAUNCHER] [VERBOSE] {message}");
        }
    }
}
