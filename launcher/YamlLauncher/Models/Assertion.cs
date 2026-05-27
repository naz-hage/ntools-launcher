#nullable enable

namespace YamlLauncher.Models;

/// <summary>
/// Defines an assertion to validate execution results.
/// </summary>
public class Assertion
{
    /// <summary>
    /// Gets or sets the type of assertion (e.g., "exitCode", "stdout", "json").
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the value to assert.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the pattern for regex-based assertions.
    /// </summary>
    public string? Pattern { get; set; }

    /// <summary>
    /// Gets or sets the JSON path for JSON-based assertions.
    /// </summary>
    public string? JsonPath { get; set; }

    /// <summary>
    /// Gets or sets the expected value for assertion comparison.
    /// </summary>
    public string? ExpectedValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the assertion is case-insensitive.
    /// </summary>
    public bool CaseInsensitive { get; set; }

    /// <summary>
    /// Gets or sets the description of the assertion.
    /// </summary>
    public string? Description { get; set; }
}
