#nullable enable

namespace YamlLauncher.Models;

/// <summary>
/// Defines variable extraction configuration from execution results.
/// </summary>
public class VariableExtraction
{
    /// <summary>
    /// Gets or sets the name of the variable to extract.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the pattern used to extract the variable value.
    /// </summary>
    public string? Pattern { get; set; }

    /// <summary>
    /// Gets or sets the regex group index for extraction (default is 0 for full match).
    /// </summary>
    public int GroupIndex { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether pattern matching is case-insensitive.
    /// </summary>
    public bool CaseInsensitive { get; set; }

    /// <summary>
    /// Gets or sets the description of the variable extraction.
    /// </summary>
    public string? Description { get; set; }
}
