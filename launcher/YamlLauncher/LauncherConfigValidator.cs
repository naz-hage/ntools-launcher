#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using YamlLauncher.Models;

namespace YamlLauncher;

/// <summary>
/// Validates LauncherConfig objects for correctness and completeness.
/// </summary>
public class LauncherConfigValidator
{
    /// <summary>
    /// Validates a launcher configuration.
    /// </summary>
    /// <param name="config">The configuration to validate.</param>
    /// <exception cref="LauncherConfigException">Thrown when validation fails.</exception>
    public void Validate(LauncherConfig config)
    {
        if (config == null)
        {
            throw new LauncherConfigException("Configuration cannot be null.");
        }

        ValidateVersion(config);
        ValidateSteps(config);
        ValidateStages(config);
        ValidateVariables(config);
        ValidateAssertions(config);
        ValidateVariableExtractions(config);
        ValidateDependencies(config);
    }

    private void ValidateVersion(LauncherConfig config)
    {
        if (string.IsNullOrWhiteSpace(config.Version))
        {
            throw new LauncherConfigException("Version field is required and cannot be empty.");
        }

        // Version should be "1.0" or similar valid format
        if (!Regex.IsMatch(config.Version, @"^\d+\.\d+(\.\d+)?$"))
        {
            throw new LauncherConfigException($"Version '{config.Version}' is not in valid format (expected format: X.Y or X.Y.Z).");
        }
    }

    private void ValidateSteps(LauncherConfig config)
    {
        if (config.Steps == null || config.Steps.Count == 0)
        {
            throw new LauncherConfigException("At least one step (or task/app) is required in the configuration.");
        }

        for (int i = 0; i < config.Steps.Count; i++)
        {
            var step = config.Steps[i];
            ValidateStep(step, i);
        }
    }

    private void ValidateStep(StepConfig step, int index)
    {
        if (step == null)
        {
            throw new LauncherConfigException($"Step at index {index} is null.");
        }

        if (string.IsNullOrWhiteSpace(step.Name))
        {
            throw new LauncherConfigException($"Step at index {index} must have a name.");
        }

        if (string.IsNullOrWhiteSpace(step.Path))
        {
            throw new LauncherConfigException($"Step '{step.Name}' must have a path to an executable.");
        }

        if (step.ExpectedReturnCode < 0)
        {
            throw new LauncherConfigException($"Step '{step.Name}' has invalid expected return code: {step.ExpectedReturnCode}. Must be >= 0.");
        }
    }

    private void ValidateStages(LauncherConfig config)
    {
        if (config.Stages == null)
        {
            return;
        }

        var stageNames = new HashSet<string>(StringComparer.Ordinal);
        for (var stageIndex = 0; stageIndex < config.Stages.Count; stageIndex++)
        {
            var stage = config.Stages[stageIndex];
            if (stage == null || string.IsNullOrWhiteSpace(stage.Name))
                throw new LauncherConfigException($"Stage at index {stageIndex} must have a name.");

            if (!stageNames.Add(stage.Name))
                throw new LauncherConfigException($"Stage name '{stage.Name}' is duplicated.");

            if (stage.Steps == null || stage.Steps.Count == 0)
                throw new LauncherConfigException($"Stage '{stage.Name}' must contain at least one step.");

            foreach (var stepName in stage.Steps)
            {
                if (string.IsNullOrWhiteSpace(stepName))
                    throw new LauncherConfigException($"Stage '{stage.Name}' contains an empty step name.");

                var matchingStepCount = config.Steps!.Count(
                    step => string.Equals(step.Name, stepName, StringComparison.Ordinal));
                if (matchingStepCount == 0)
                    throw new LauncherConfigException(
                        $"Stage '{stage.Name}' references step '{stepName}', but no such step exists.");

                if (matchingStepCount > 1)
                    throw new LauncherConfigException(
                        $"Stage '{stage.Name}' references ambiguous step '{stepName}'.");
            }
        }
    }

    private void ValidateVariables(LauncherConfig config)
    {
        if (config.Variables == null || config.Variables.Count == 0)
        {
            return; // Variables are optional
        }

        foreach (var kvp in config.Variables)
        {
            if (string.IsNullOrWhiteSpace(kvp.Key))
            {
                throw new LauncherConfigException("Variable key cannot be empty.");
            }

            if (kvp.Value == null)
            {
                throw new LauncherConfigException($"Variable '{kvp.Key}' has null value.");
            }
        }
    }

    private void ValidateAssertions(LauncherConfig config)
    {
        if (config.Steps == null)
        {
            return;
        }

        foreach (var step in config.Steps)
        {
            if (step.Assertions == null || step.Assertions.Count == 0)
            {
                continue;
            }

            for (int i = 0; i < step.Assertions.Count; i++)
            {
                var assertion = step.Assertions[i];
                ValidateAssertion(assertion, step.Name ?? "unknown", i);
            }
        }
    }

    private void ValidateAssertion(Assertion assertion, string stepName, int index)
    {
        if (assertion == null)
        {
            throw new LauncherConfigException($"Assertion at index {index} in step '{stepName}' is null.");
        }

        if (string.IsNullOrWhiteSpace(assertion.Type))
        {
            throw new LauncherConfigException($"Assertion at index {index} in step '{stepName}' must have a type.");
        }

        // If pattern is specified, validate it as a regex
        if (!string.IsNullOrWhiteSpace(assertion.Pattern))
        {
            try
            {
                _ = new Regex(assertion.Pattern);
            }
            catch (ArgumentException ex)
            {
                throw new LauncherConfigException(
                    $"Assertion at index {index} in step '{stepName}' has invalid regex pattern: {assertion.Pattern}",
                    ex);
            }
        }
    }

    private void ValidateVariableExtractions(LauncherConfig config)
    {
        if (config.Steps == null)
        {
            return;
        }

        foreach (var step in config.Steps)
        {
            if (step.ExtractVariables == null || step.ExtractVariables.Count == 0)
            {
                continue;
            }

            for (int i = 0; i < step.ExtractVariables.Count; i++)
            {
                var extraction = step.ExtractVariables[i];
                ValidateVariableExtraction(extraction, step.Name ?? "unknown", i);
            }
        }
    }

    private void ValidateVariableExtraction(VariableExtraction extraction, string stepName, int index)
    {
        if (extraction == null)
        {
            throw new LauncherConfigException($"Variable extraction at index {index} in step '{stepName}' is null.");
        }

        if (string.IsNullOrWhiteSpace(extraction.Name))
        {
            throw new LauncherConfigException($"Variable extraction at index {index} in step '{stepName}' must have a name.");
        }

        if (string.IsNullOrWhiteSpace(extraction.Pattern))
        {
            throw new LauncherConfigException($"Variable extraction '{extraction.Name}' in step '{stepName}' must have a pattern.");
        }

        if (extraction.GroupIndex < 0)
        {
            throw new LauncherConfigException($"Variable extraction '{extraction.Name}' in step '{stepName}' has invalid group index: {extraction.GroupIndex}. Must be >= 0.");
        }

        // Validate regex pattern
        try
        {
            _ = new Regex(extraction.Pattern);
        }
        catch (ArgumentException ex)
        {
            throw new LauncherConfigException(
                $"Variable extraction '{extraction.Name}' in step '{stepName}' has invalid regex pattern: {extraction.Pattern}",
                ex);
        }
    }

    private void ValidateDependencies(LauncherConfig config)
    {
        if (config.Steps == null || config.Steps.Count == 0)
        {
            return;
        }

        var stepNames = new HashSet<string>(config.Steps.Select(s => s.Name ?? "").Where(n => !string.IsNullOrWhiteSpace(n)));

        foreach (var step in config.Steps)
        {
            if (step.Dependencies == null || step.Dependencies.Count == 0)
            {
                continue;
            }

            foreach (var dependency in step.Dependencies)
            {
                if (!stepNames.Contains(dependency))
                {
                    throw new LauncherConfigException(
                        $"Step '{step.Name}' has dependency on '{dependency}', but no such step exists.");
                }
            }
        }
    }
}
