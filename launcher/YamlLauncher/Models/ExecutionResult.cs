#nullable enable

using System.Collections.Generic;

namespace YamlLauncher.Models;

/// <summary>
/// Represents the result of a single step execution.
/// </summary>
public class ExecutionResult
{
    /// <summary>
    /// Gets or sets the name of the executed step.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the step executed successfully.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the exit code returned by the executed process.
    /// </summary>
    public int ExitCode { get; set; }

    /// <summary>
    /// Gets or sets the standard output from the executed process.
    /// </summary>
    public string? StandardOutput { get; set; }

    /// <summary>
    /// Gets or sets the standard error output from the executed process.
    /// </summary>
    public string? StandardError { get; set; }

    /// <summary>
    /// Gets or sets the results of all assertions validated for this step.
    /// </summary>
    public List<AssertionResult>? Assertions { get; set; }

    /// <summary>
    /// Gets or sets the variables extracted from this step's execution results.
    /// </summary>
    public List<ExtractionResult>? ExtractedVariables { get; set; }

    /// <summary>
    /// Gets or sets the error message if the step failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
