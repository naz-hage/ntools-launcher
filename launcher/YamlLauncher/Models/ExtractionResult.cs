#nullable enable

namespace YamlLauncher.Models;

/// <summary>
/// Represents the result of a variable extraction operation.
/// </summary>
public class ExtractionResult
{
    /// <summary>
    /// Gets or sets the name of the extracted variable.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the extracted value.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the extraction was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the message describing the extraction result.
    /// </summary>
    public string? Message { get; set; }
}
