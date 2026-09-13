#nullable enable

using System;

namespace YamlLauncher.Logging;

/// <summary>
/// Standard implementation of ILogger for ntools-launcher.
/// Outputs messages with appropriate log level prefixes ([LAUNCHER] [INFO], [LAUNCHER] [WARN], [LAUNCHER] [ERROR], [LAUNCHER] [VERBOSE]).
/// </summary>
public class Logger : ILogger
{
    private readonly bool _verbose;
    private readonly string _component;

    /// <summary>
    /// Creates a new Logger instance.
    /// </summary>
    /// <param name="verbose">Whether to enable verbose/debug logging.</param>
    /// <param name="component">Component name for log prefix (default: LAUNCHER).</param>
    public Logger(bool verbose = false, string component = "LAUNCHER")
    {
        _verbose = verbose;
        _component = component;
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
        Console.WriteLine($"[{_component}] [INFO] {message}");
    }

    /// <summary>
    /// Logs a warning message to stderr.
    /// </summary>
    /// <param name="message">The warning message to log.</param>
    public void LogWarning(string message)
    {
        Console.Error.WriteLine($"[{_component}] [WARN] {message}");
    }

    /// <summary>
    /// Logs an error message to stderr.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    public void LogError(string message)
    {
        Console.Error.WriteLine($"[{_component}] [ERROR] {message}");
    }

    /// <summary>
    /// Logs a verbose/debug message to stdout if verbose mode is enabled.
    /// </summary>
    /// <param name="message">The verbose message to log.</param>
    public void LogVerbose(string message)
    {
        if (_verbose)
        {
            Console.WriteLine($"[{_component}] [VERBOSE] {message}");
        }
    }
}
