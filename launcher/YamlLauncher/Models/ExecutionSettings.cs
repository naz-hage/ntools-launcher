#nullable enable

using System;

namespace YamlLauncher.Models;

/// <summary>
/// Defines execution mode options.
/// </summary>
public enum ExecutionMode
{
    /// <summary>
    /// Execute steps sequentially.
    /// </summary>
    Sequential = 0,

    /// <summary>
    /// Execute steps in parallel.
    /// </summary>
    Parallel = 1
}

/// <summary>
/// Defines execution settings for the YAML launcher.
/// </summary>
public class ExecutionSettings
{
    /// <summary>
    /// Gets or sets the execution mode (Sequential or Parallel).
    /// </summary>
    public ExecutionMode Mode { get; set; } = ExecutionMode.Sequential;

    /// <summary>
    /// Gets or sets a value indicating whether verbose logging is enabled.
    /// </summary>
    public bool Verbose { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether execution should stop on the first error.
    /// </summary>
    public bool StopOnFirstError { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of steps to execute concurrently (only used in Parallel mode).
    /// </summary>
    public int MaxConcurrency { get; set; } = Environment.ProcessorCount;

    /// <summary>
    /// Gets or sets the timeout in seconds for the entire execution (0 = no timeout).
    /// </summary>
    public int Timeout { get; set; } = 0;
}
