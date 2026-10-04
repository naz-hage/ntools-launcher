#nullable enable

using System.Collections.Generic;

namespace YamlLauncher.Models;

/// <summary>
/// Defines a named, ordered group of configured steps.
/// </summary>
public class StageConfig
{
    /// <summary>
    /// Gets or sets the stage name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the names of the steps to execute in order.
    /// </summary>
    public List<string>? Steps { get; set; }
}
