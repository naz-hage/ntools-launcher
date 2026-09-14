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

    /// <summary>
    /// Gets or sets the duration of step execution in milliseconds.
    /// </summary>
    public long DurationMs { get; set; }

    // Convenience properties for compatibility

    /// <summary>
    /// Gets or sets the step name (alias for Name).
    /// </summary>
    public string? StepName 
    { 
        get => Name; 
        set => Name = value; 
    }

    /// <summary>
    /// Gets or sets standard output (alias for StandardOutput).
    /// </summary>
    public string? StdOut 
    { 
        get => StandardOutput; 
        set => StandardOutput = value; 
    }

    /// <summary>
    /// Gets or sets standard error output (alias for StandardError).
    /// </summary>
    public string? StdErr 
    { 
        get => StandardError; 
        set => StandardError = value; 
    }
}
