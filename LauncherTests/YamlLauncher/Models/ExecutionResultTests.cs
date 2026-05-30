using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YamlLauncher.Models;

namespace LauncherTests.YamlLauncher.Models;

[TestClass]
public class ExecutionResultTests
{
    [TestMethod]
    public void Constructor_InitializesWithDefaults()
    {
        // Act
        var result = new ExecutionResult();

        // Assert
        Assert.IsNull(result.Name);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(0, result.ExitCode);
        Assert.IsNull(result.StandardOutput);
        Assert.IsNull(result.StandardError);
        Assert.IsNull(result.Assertions);
        Assert.IsNull(result.ExtractedVariables);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public void Properties_CanBeSet()
    {
        // Arrange
        var result = new ExecutionResult();

        // Act
        result.Name = "test-step";
        result.Success = true;
        result.ExitCode = 0;
        result.StandardOutput = "Test output";
        result.StandardError = null;
        result.ErrorMessage = null;

        // Assert
        Assert.AreEqual("test-step", result.Name);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.ExitCode);
        Assert.AreEqual("Test output", result.StandardOutput);
    }

    [TestMethod]
    public void Assertions_CanBeAdded()
    {
        // Arrange & Act
        var result = new ExecutionResult
        {
            Name = "build",
            Assertions = new List<AssertionResult>
            {
                new AssertionResult { Type = "exitCode", Passed = true },
                new AssertionResult { Type = "stdout", Passed = true }
            }
        };

        // Assert
        Assert.IsNotNull(result.Assertions);
        Assert.AreEqual(2, result.Assertions.Count);
    }

    [TestMethod]
    public void ExtractedVariables_CanBeAdded()
    {
        // Arrange & Act
        var result = new ExecutionResult
        {
            Name = "deploy",
            ExtractedVariables = new List<ExtractionResult>
            {
                new ExtractionResult { Name = "deploymentId", Value = "abc123" },
                new ExtractionResult { Name = "url", Value = "https://example.com" }
            }
        };

        // Assert
        Assert.IsNotNull(result.ExtractedVariables);
        Assert.AreEqual(2, result.ExtractedVariables.Count);
    }

    [TestMethod]
    public void ExecutionResult_SuccessfulScenario()
    {
        // Arrange & Act
        var result = new ExecutionResult
        {
            Name = "build-step",
            Success = true,
            ExitCode = 0,
            StandardOutput = "Build succeeded",
            StandardError = null,
            Assertions = new List<AssertionResult>
            {
                new AssertionResult { Type = "exitCode", Passed = true, Message = "Exit code 0" }
            }
        };

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.ExitCode);
        Assert.IsNull(result.ErrorMessage);
        Assert.IsTrue(result.Assertions[0].Passed);
    }

    [TestMethod]
    public void ExecutionResult_FailureScenario()
    {
        // Arrange & Act
        var result = new ExecutionResult
        {
            Name = "test-step",
            Success = false,
            ExitCode = 1,
            StandardOutput = "Test failed",
            StandardError = "Error details",
            ErrorMessage = "Test execution failed with exit code 1"
        };

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(1, result.ExitCode);
        Assert.IsNotNull(result.ErrorMessage);
    }

    [TestMethod]
    public void ExecutionResult_ComplexScenario()
    {
        // Arrange & Act
        var result = new ExecutionResult
        {
            Name = "integration-test",
            Success = true,
            ExitCode = 0,
            StandardOutput = "All tests passed: 42/42",
            StandardError = "Warning: Deprecated API used",
            Assertions = new List<AssertionResult>
            {
                new AssertionResult { Type = "exitCode", Passed = true, ExpectedValue = "0", ActualValue = "0" },
                new AssertionResult { Type = "stdout", Passed = true, Message = "All tests passed" }
            },
            ExtractedVariables = new List<ExtractionResult>
            {
                new ExtractionResult { Name = "testCount", Value = "42", Success = true },
                new ExtractionResult { Name = "testStatus", Value = "passed", Success = true }
            }
        };

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Assertions.Count);
        Assert.AreEqual(2, result.ExtractedVariables.Count);
    }

    [TestMethod]
    public void ExitCode_CanHaveNonZeroValue()
    {
        // Arrange & Act
        var result = new ExecutionResult
        {
            Name = "failed-command",
            ExitCode = 127
        };

        // Assert
        Assert.AreEqual(127, result.ExitCode);
    }
}
