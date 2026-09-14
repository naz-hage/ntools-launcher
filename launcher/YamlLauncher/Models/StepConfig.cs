#nullable enable

using System.Collections.Generic;

namespace YamlLauncher.Models;

/// <summary>
/// Defines a single step/task/app configuration in the YAML launcher.
/// Replaces ExecutableConfig in the model hierarchy.
/// </summary>
public class StepConfig
{
    /// <summary>
    /// Gets or sets the path to the executable or script to run.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets the command-line arguments to pass to the executable.
    /// </summary>
    public string? Arguments { get; set; }

    /// <summary>
    /// Gets or sets the name/identifier of the step.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the list of step names this step depends on (for orchestration).
    /// </summary>
    public List<string>? Dependencies { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether execution should continue even if this step fails.
    /// </summary>
    public bool ContinueOnError { get; set; }

    /// <summary>
    /// Gets or sets the expected return code for a successful execution.
    /// </summary>
    public int ExpectedReturnCode { get; set; } = 0;

    /// <summary>
    /// Gets or sets the list of assertions to validate the execution results.
    /// </summary>
    public List<Assertion>? Assertions { get; set; }

    /// <summary>
    /// Gets or sets the list of variables to extract from the execution results.
    /// </summary>
    public List<VariableExtraction>? ExtractVariables { get; set; }

    /// <summary>
    /// Gets or sets the working directory for process execution.
    /// </summary>
    public string? WorkingDirectory { get; set; }

    /// <summary>
    /// Gets or sets step-specific environment variables to pass to the process.
    /// These are merged with global variables from the LauncherConfig.
    /// </summary>
    public Dictionary<string, string>? Environment { get; set; }

    /// <summary>
    /// Gets or sets the required digital signature for the executable (Windows only).
    /// If specified, the executable's digital signature must match for execution to proceed.
    /// </summary>
    public string? RequireSignature { get; set; }
}
