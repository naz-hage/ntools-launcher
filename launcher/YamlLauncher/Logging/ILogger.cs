#nullable enable

using System;

namespace YamlLauncher.Logging;

/// <summary>
/// Interface for comprehensive logging across ntools-launcher.
/// Provides methods for different log levels with optional verbosity control.
/// </summary>
public interface ILogger
{
    /// <summary>
    /// Gets a value indicating whether verbose logging is enabled.
    /// </summary>
    bool IsVerbose { get; }

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogInfo(string message);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The warning message to log.</param>
    void LogWarning(string message);

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    void LogError(string message);

    /// <summary>
    /// Logs a verbose/debug message (only shown when verbose mode is enabled).
    /// </summary>
    /// <param name="message">The verbose message to log.</param>
    void LogVerbose(string message);
}
