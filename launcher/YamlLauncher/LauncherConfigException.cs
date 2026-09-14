#nullable enable

using System;

namespace YamlLauncher;

/// <summary>
/// Exception thrown when a launcher configuration is invalid.
/// </summary>
public class LauncherConfigException : Exception
{
    /// <summary>
    /// Initializes a new instance of the LauncherConfigException class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public LauncherConfigException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the LauncherConfigException class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public LauncherConfigException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Gets or sets the line number where the error occurred (if available).
    /// </summary>
    public int? LineNumber { get; set; }

    /// <summary>
    /// Gets or sets the column number where the error occurred (if available).
    /// </summary>
    public int? ColumnNumber { get; set; }
}
