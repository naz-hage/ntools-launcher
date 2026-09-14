#nullable enable

using System;
using System.Collections.Generic;

namespace YamlLauncher.Models;

/// <summary>
/// Represents the overall result of a launcher execution.
/// </summary>
public class LaunchResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the entire launcher execution was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the list of execution results for each step.
    /// </summary>
    public List<ExecutionResult>? Results { get; set; }

    /// <summary>
    /// Gets or sets the variables extracted during the entire execution.
    /// </summary>
    public Dictionary<string, string>? ExtractedVariables { get; set; }

    /// <summary>
    /// Gets or sets the error message if the execution failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the UTC start time of the execution.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Gets or sets the UTC end time of the execution.
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Gets or sets the total duration of the execution in milliseconds.
    /// </summary>
    public long DurationMs { get; set; }
}
