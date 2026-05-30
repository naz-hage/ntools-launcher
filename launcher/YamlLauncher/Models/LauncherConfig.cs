#nullable enable

using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace YamlLauncher.Models;

/// <summary>
/// Main configuration model for the YAML launcher.
/// Represents the complete configuration loaded from YAML files.
/// </summary>
public class LauncherConfig
{
    /// <summary>
    /// Gets or sets the version of the launcher configuration format.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets the description of this configuration.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the execution settings for this configuration.
    /// </summary>
    public ExecutionSettings? Execution { get; set; }

    /// <summary>
    /// Gets or sets the variables/environment variables to use during execution.
    /// </summary>
    public Dictionary<string, string>? Variables { get; set; }

    /// <summary>
    /// Gets or sets the canonical list of steps (also accessible via Tasks and Apps aliases).
    /// </summary>
    public List<StepConfig>? Steps { get; set; }

    /// <summary>
    /// Gets or sets the tasks (alias for Steps, used for test-framework integration context).
    /// </summary>
    [YamlIgnore]
    public List<StepConfig>? Tasks
    {
        get => Steps;
        set => Steps = value;
    }

    /// <summary>
    /// Gets or sets the apps (alias for Steps, used for nb integration context).
    /// </summary>
    [YamlIgnore]
    public List<StepConfig>? Apps
    {
        get => Steps;
        set => Steps = value;
    }
}
