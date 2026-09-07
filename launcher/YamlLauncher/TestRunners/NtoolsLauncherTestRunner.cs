#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YamlLauncher.Logging;
using YamlLauncher.Models;

namespace YamlLauncher.TestRunners;

/// <summary>
/// Test runner for ntools-launcher YAML configuration files.
/// Loads YAML configurations, validates them, executes steps, and extracts variables from output.
/// </summary>
public class NtoolsLauncherTestRunner
{
    private readonly bool _verbose;
    private readonly string _metadataPath;
    private readonly ILogger _logger;

    public NtoolsLauncherTestRunner(bool verbose = false, ILogger? logger = null, string? metadataPath = null)
    {
        _verbose = verbose;
        _metadataPath = metadataPath ?? Path.Combine(AppContext.BaseDirectory, "metadata");
        _logger = logger ?? new Logger(verbose, "LAUNCHER");
    }

    /// <summary>
    /// Runs a test by name from the ntools-launcher YAML metadata.
    /// </summary>
    public async Task<bool> RunTestAsync(string testName)
    {
        try
        {
            var yamlFile = FindYamlFile(testName);

            if (yamlFile == null)
            {
                _logger.LogError($"Test YAML file not found for test: {testName}");
                return false;
            }

            _logger.LogInfo($"{'='} Running ntools-launcher Test: {testName} {'='}");

            // Load the YAML configuration
            var loader = new YamlLauncherConfigLoader();
            LauncherConfig config;

            try
            {
                config = await loader.LoadFromFileAsync(yamlFile);
                _logger.LogInfo("[✓] YAML Configuration loaded successfully");
                _logger.LogInfo($"    Version: {config.Version}");
                _logger.LogInfo($"    Steps: {config.Steps?.Count ?? 0}");
                _logger.LogInfo($"    Description: {config.Description}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load YAML: {ex.Message}");
                if (ex is LauncherConfigException lcex)
                {
                    _logger.LogError($"    Line: {lcex.LineNumber}, Column: {lcex.ColumnNumber}");
                }
                return false;
            }

            // Validate the configuration
            _logger.LogInfo("--- Configuration Validation ---");
            try
            {
                var validator = new LauncherConfigValidator();
                validator.Validate(config);
                _logger.LogInfo("[✓] Configuration is valid");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Configuration validation failed: {ex.Message}");
                return false;
            }

            // Execute the steps
            _logger.LogInfo("--- Executing Steps ---");
            var executor = new StepExecutor(verbose: _verbose, logger: _logger);
            bool allSuccess = true;
            var variables = new Dictionary<string, string>();

            if (config.Steps != null)
            {
                for (int i = 0; i < config.Steps.Count; i++)
                {
                    var step = config.Steps[i];
                        
                    // Substitute variables in arguments for this step
                    if (variables.Count > 0 && !string.IsNullOrEmpty(step.Arguments))
                    {
                        step.Arguments = SubstituteVariables(step.Arguments, variables);
                    }

                    try
                    {
                        var result = await executor.LaunchAsync(config, i);

                        // Get the execution result - it should be the last one in the Results collection
                        var executionResult = result.Results?.LastOrDefault();
                        if (executionResult == null)
                        {
                            _logger.LogError("No execution result returned");
                            allSuccess = false;
                            continue;
                        }

                        // Extract variables from step output
                        ExtractVariables(step, executionResult, variables);

                        if (!EvaluateAssertions(step, executionResult))
                        {
                            allSuccess = false;
                        }

                        _logger.LogInfo($"Exit Code: {executionResult.ExitCode}");
                        _logger.LogInfo($"Duration: {executionResult.DurationMs}ms");
                        
                        if (executionResult.Success)
                        {
                            _logger.LogInfo("[✓] Step succeeded");
                        }
                        else
                        {
                            _logger.LogError("[X] Step failed");
                            allSuccess = false;
                        }

                        // Show extracted variables
                        if (executionResult.ExtractedVariables?.Count > 0)
                        {
                            _logger.LogInfo("Variables extracted:");
                            foreach (var varResult in executionResult.ExtractedVariables)
                            {
                                _logger.LogInfo($"  - {varResult.Name} = {varResult.Value}");
                            }
                        }

                        // Show stdout/stderr if verbose
                        if (_verbose)
                        {
                            if (!string.IsNullOrEmpty(executionResult.StandardOutput))
                            {
                                _logger.LogVerbose($"Output:\n{string.Join("\n", executionResult.StandardOutput.Split('\n').Take(5))}");
                            }
                            if (!string.IsNullOrEmpty(executionResult.StandardError))
                            {
                                _logger.LogVerbose($"Error:\n{string.Join("\n", executionResult.StandardError.Split('\n').Take(5))}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Execution failed: {ex.Message}");
                        allSuccess = false;
                    }
                }
            }

            // Summary
            _logger.LogInfo("--- Execution Summary ---");
            if (allSuccess)
            {
                _logger.LogInfo("[✓] All steps executed successfully");
                return true;
            }
            else
            {
                _logger.LogError("[X] Some steps failed");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Runs all discovered YAML test configurations in deterministic name order.
    /// </summary>
    public async Task<bool> RunAllTestsAsync()
    {
        var testNames = GetYamlTestFiles()
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => !string.IsNullOrEmpty(name))
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        if (testNames.Length == 0)
        {
            _logger.LogInfo("No YAML test files found");
            return true;
        }

        var allSuccess = true;
        foreach (var testName in testNames)
        {
            if (!await RunTestAsync(testName!))
            {
                allSuccess = false;
            }
        }

        return allSuccess;
    }

    /// <summary>
    /// Lists all available ntools-launcher YAML test files.
    /// </summary>
    public void ListTests()
    {
        try
        {
            if (!Directory.Exists(_metadataPath))
            {
                _logger.LogInfo("No metadata directory found");
                return;
            }

            var yamlFiles = GetYamlTestFiles();

            if (yamlFiles.Length == 0)
            {
                _logger.LogInfo("No YAML test files found");
                return;
            }

            _logger.LogInfo($"Available tests ({yamlFiles.Length}):");

            foreach (var file in yamlFiles.OrderBy(file => file, StringComparer.Ordinal))
            {
                var testName = Path.GetFileNameWithoutExtension(file);
                _logger.LogInfo($"  • {testName}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error listing tests: {ex.Message}");
        }
    }

    private string? FindYamlFile(string testName)
    {
        foreach (var extension in new[] { ".yaml", ".ntools.yml", ".yml" })
        {
            var candidate = Path.Combine(_metadataPath, $"{testName}{extension}");
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    private string[] GetYamlTestFiles()
    {
        if (!Directory.Exists(_metadataPath))
        {
            return Array.Empty<string>();
        }

        return Directory.GetFiles(_metadataPath)
            .Where(file =>
            {
                var fileName = Path.GetFileName(file);
                return fileName.StartsWith("Test_", StringComparison.OrdinalIgnoreCase) &&
                    (fileName.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase) ||
                     fileName.EndsWith(".yml", StringComparison.OrdinalIgnoreCase)) ||
                    fileName.StartsWith("Validate_", StringComparison.OrdinalIgnoreCase) &&
                    fileName.EndsWith(".yml", StringComparison.OrdinalIgnoreCase);
            })
            .ToArray();
    }

    /// <summary>
    /// Substitutes variables in text using {varName} syntax.
    /// </summary>
    private string SubstituteVariables(string text, Dictionary<string, string> variables)
    {
        if (string.IsNullOrEmpty(text) || variables.Count == 0)
            return text;

        var result = text;
        foreach (var variable in variables)
        {
            result = result.Replace($"{{{variable.Key}}}", variable.Value);
        }
        return result;
    }

    /// <summary>
    /// Extracts variables from step execution output using configured patterns.
    /// Based on test-framework MetadataTestExecutor logic with bounds checking.
    /// </summary>
    private void ExtractVariables(StepConfig step, ExecutionResult result, Dictionary<string, string> variables)
    {
        // Check if there are any variable extractions configured
        if (step.ExtractVariables == null || step.ExtractVariables.Count == 0)
        {
            _logger.LogVerbose("No variable extractions configured for this step");
            return;
        }

        var output = result.StandardOutput ?? string.Empty;
        
        if (string.IsNullOrEmpty(output))
        {
            _logger.LogVerbose("No output to extract variables from");
            return;
        }

        _logger.LogVerbose($"Extracting {step.ExtractVariables.Count} variable(s)");

        foreach (var extraction in step.ExtractVariables)
        {
            try
            {
                if (string.IsNullOrEmpty(extraction.Pattern))
                {
                    _logger.LogError($"Extraction '{extraction.Name}': No pattern defined");
                    continue;
                }

                if (string.IsNullOrEmpty(extraction.Name))
                {
                    _logger.LogError("Extraction has no variable name defined");
                    continue;
                }

                _logger.LogVerbose($"Pattern from YAML: '{extraction.Pattern}' (length={extraction.Pattern.Length})");
                _logger.LogVerbose($"Pattern bytes: {string.Join(", ", extraction.Pattern.Select(c => $"'{c}'({(int)c})"))}");
                _logger.LogVerbose($"Attempting to extract '{extraction.Name}' using pattern: {extraction.Pattern}, groupIndex: {extraction.GroupIndex}");

                var regexOptions = extraction.CaseInsensitive ? RegexOptions.IgnoreCase | RegexOptions.Multiline : RegexOptions.Multiline;
                var match = Regex.Match(output, extraction.Pattern, regexOptions);

                if (match.Success)
                {
                    _logger.LogVerbose($"Match found! Groups count: {match.Groups.Count}");
                    _logger.LogVerbose($"Match position: Index={match.Index}, Length={match.Length}, Value='{match.Value}'");
                    _logger.LogVerbose($"Context: '{output.Substring(Math.Max(0, match.Index - 20), Math.Min(60, output.Length - match.Index + 20))}'");
                    for (int g = 0; g < match.Groups.Count; g++)
                    {
                        _logger.LogVerbose($"  Group[{g}] = '{match.Groups[g].Value}'");
                    }
                    _logger.LogVerbose($"Full output length: {output.Length}");
                    _logger.LogVerbose($"Full output:\n{output}");

                    // **CRITICAL FIX**: Check bounds before accessing group, just like test-framework does
                    var value = match.Groups.Count > extraction.GroupIndex
                        ? match.Groups[extraction.GroupIndex].Value
                        : match.Value;

                    if (!string.IsNullOrEmpty(value))
                    {
                        variables[extraction.Name] = value;
                        _logger.LogInfo($"[✓] Extracted: {extraction.Name} = {value}");
                    }
                    else
                    {
                        _logger.LogInfo($"[·] Pattern matched but extracted value is empty");
                    }
                }
                else
                {
                    _logger.LogError($"[X] Pattern did not match output");
                    if (_verbose)
                    {
                        var preview = output.Length > 200 ? output.Substring(0, 200) + "..." : output;
                        _logger.LogVerbose($"   Output: {preview}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[X] Error extracting variable {extraction.Name}: {ex.Message}");
            }
        }
    }

    private bool EvaluateAssertions(StepConfig step, ExecutionResult result)
    {
        if (step.Assertions == null || step.Assertions.Count == 0)
        {
            return true;
        }

        var output = string.Concat(result.StandardOutput, result.StandardError);
        var allPassed = true;

        foreach (var assertion in step.Assertions)
        {
            var comparison = assertion.CaseInsensitive
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
            var type = assertion.Type?.Trim().ToLowerInvariant();
            var passed = type switch
            {
                "exit_code" => int.TryParse(assertion.Value, out var expectedExitCode) &&
                    result.ExitCode == expectedExitCode,
                "output_contains" => assertion.Value != null &&
                    output.Contains(assertion.Value, comparison),
                "output_matches" => !string.IsNullOrEmpty(assertion.Pattern) &&
                    Regex.IsMatch(
                        output,
                        assertion.Pattern,
                        assertion.CaseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None),
                _ => false
            };

            if (passed)
            {
                _logger.LogInfo($"[✓] Assertion passed: {assertion.Type}");
            }
            else
            {
                var expected = assertion.Type?.Trim().Equals("output_matches", StringComparison.OrdinalIgnoreCase) == true
                    ? assertion.Pattern ?? "<missing>"
                    : assertion.Value ?? "<missing>";
                _logger.LogError($"[X] Assertion failed: {assertion.Type}; expected '{expected}'");
                allPassed = false;
            }
        }

        return allPassed;
    }
}
