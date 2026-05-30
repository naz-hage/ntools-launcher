using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YamlLauncher.Models;

namespace LauncherTests.YamlLauncher.Models;

[TestClass]
public class LaunchResultTests
{
    [TestMethod]
    public void Constructor_InitializesWithDefaults()
    {
        // Act
        var result = new LaunchResult();

        // Assert
        Assert.IsFalse(result.Success);
        Assert.IsNull(result.Results);
        Assert.IsNull(result.ExtractedVariables);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public void Properties_CanBeSet()
    {
        // Arrange
        var result = new LaunchResult();

        // Act
        result.Success = true;
        result.Results = new List<ExecutionResult>();
        result.ExtractedVariables = new Dictionary<string, string>();
        result.ErrorMessage = null;

        // Assert
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Results);
        Assert.IsNotNull(result.ExtractedVariables);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public void Results_CanContainMultipleExecutionResults()
    {
        // Arrange & Act
        var result = new LaunchResult
        {
            Results = new List<ExecutionResult>
            {
                new ExecutionResult { Name = "step1", Success = true },
                new ExecutionResult { Name = "step2", Success = true },
                new ExecutionResult { Name = "step3", Success = false }
            }
        };

        // Assert
        Assert.AreEqual(3, result.Results.Count);
        Assert.AreEqual("step1", result.Results[0].Name);
        Assert.AreEqual("step3", result.Results[2].Name);
    }

    [TestMethod]
    public void ExtractedVariables_CanContainMultipleEntries()
    {
        // Arrange & Act
        var result = new LaunchResult
        {
            ExtractedVariables = new Dictionary<string, string>
            {
                { "version", "1.2.3" },
                { "buildId", "12345" },
                { "deploymentUrl", "https://example.com" }
            }
        };

        // Assert
        Assert.AreEqual(3, result.ExtractedVariables.Count);
        Assert.AreEqual("1.2.3", result.ExtractedVariables["version"]);
        Assert.AreEqual("https://example.com", result.ExtractedVariables["deploymentUrl"]);
    }

    [TestMethod]
    public void LaunchResult_SuccessfulScenario()
    {
        // Arrange & Act
        var result = new LaunchResult
        {
            Success = true,
            Results = new List<ExecutionResult>
            {
                new ExecutionResult { Name = "step1", Success = true, ExitCode = 0 },
                new ExecutionResult { Name = "step2", Success = true, ExitCode = 0 }
            },
            ExtractedVariables = new Dictionary<string, string>
            {
                { "output", "deployment successful" }
            },
            ErrorMessage = null
        };

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Results.Count);
        Assert.AreEqual(1, result.ExtractedVariables.Count);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public void LaunchResult_FailureScenario()
    {
        // Arrange & Act
        var result = new LaunchResult
        {
            Success = false,
            Results = new List<ExecutionResult>
            {
                new ExecutionResult { Name = "step1", Success = true, ExitCode = 0 },
                new ExecutionResult { Name = "step2", Success = false, ExitCode = 1 }
            },
            ExtractedVariables = new Dictionary<string, string>(),
            ErrorMessage = "Execution failed at step2"
        };

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(2, result.Results.Count);
        Assert.AreEqual("Execution failed at step2", result.ErrorMessage);
    }

    [TestMethod]
    public void LaunchResult_ComplexScenario()
    {
        // Arrange & Act
        var launchResult = new LaunchResult
        {
            Success = true,
            Results = new List<ExecutionResult>
            {
                new ExecutionResult
                {
                    Name = "restore",
                    Success = true,
                    ExitCode = 0,
                    StandardOutput = "Restore completed",
                    Assertions = new List<AssertionResult>
                    {
                        new AssertionResult { Type = "exitCode", Passed = true }
                    }
                },
                new ExecutionResult
                {
                    Name = "build",
                    Success = true,
                    ExitCode = 0,
                    StandardOutput = "Build succeeded",
                    ExtractedVariables = new List<ExtractionResult>
                    {
                        new ExtractionResult { Name = "buildDir", Value = "/build/output", Success = true }
                    },
                    Assertions = new List<AssertionResult>
                    {
                        new AssertionResult { Type = "exitCode", Passed = true },
                        new AssertionResult { Type = "stdout", Passed = true }
                    }
                }
            },
            ExtractedVariables = new Dictionary<string, string>
            {
                { "buildDir", "/build/output" },
                { "version", "1.0.0" }
            }
        };

        // Assert
        Assert.IsTrue(launchResult.Success);
        Assert.AreEqual(2, launchResult.Results.Count);
        Assert.AreEqual(2, launchResult.ExtractedVariables.Count);
        Assert.IsTrue(launchResult.Results[0].Assertions![0].Passed);
        Assert.AreEqual(2, launchResult.Results[1].Assertions!.Count);
    }

    [TestMethod]
    public void Results_DefaultsToNull()
    {
        // Arrange & Act
        var result = new LaunchResult();

        // Assert
        Assert.IsNull(result.Results);
    }

    [TestMethod]
    public void ExtractedVariables_DefaultsToNull()
    {
        // Arrange & Act
        var result = new LaunchResult();

        // Assert
        Assert.IsNull(result.ExtractedVariables);
    }

    [TestMethod]
    public void Success_DefaultsFalse()
    {
        // Arrange & Act
        var result = new LaunchResult();

        // Assert
        Assert.IsFalse(result.Success);
    }
}
