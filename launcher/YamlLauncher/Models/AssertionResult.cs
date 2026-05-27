#nullable enable

namespace YamlLauncher.Models;

/// <summary>
/// Represents the result of an individual assertion validation.
/// </summary>
public class AssertionResult
{
    /// <summary>
    /// Gets or sets the type of assertion that was validated.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the assertion passed.
    /// </summary>
    public bool Passed { get; set; }

    /// <summary>
    /// Gets or sets the message describing the assertion result.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the expected value from the assertion.
    /// </summary>
    public string? ExpectedValue { get; set; }

    /// <summary>
    /// Gets or sets the actual value found during validation.
    /// </summary>
    public string? ActualValue { get; set; }
}
