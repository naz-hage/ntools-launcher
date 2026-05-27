using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YamlLauncher.Models;

namespace LauncherTests.YamlLauncher.Models;

[TestClass]
public class AssertionResultTests
{
    [TestMethod]
    public void Constructor_InitializesWithDefaults()
    {
        // Act
        var result = new AssertionResult();

        // Assert
        Assert.IsNull(result.Type);
        Assert.IsFalse(result.Passed);
        Assert.IsNull(result.Message);
        Assert.IsNull(result.ExpectedValue);
        Assert.IsNull(result.ActualValue);
    }

    [TestMethod]
    public void Properties_CanBeSet()
    {
        // Arrange
        var result = new AssertionResult();

        // Act
        result.Type = "exitCode";
        result.Passed = true;
        result.Message = "Assertion passed";
        result.ExpectedValue = "0";
        result.ActualValue = "0";

        // Assert
        Assert.AreEqual("exitCode", result.Type);
        Assert.IsTrue(result.Passed);
        Assert.AreEqual("Assertion passed", result.Message);
        Assert.AreEqual("0", result.ExpectedValue);
        Assert.AreEqual("0", result.ActualValue);
    }

    [TestMethod]
    public void AssertionResult_PassedScenario()
    {
        // Arrange & Act
        var result = new AssertionResult
        {
            Type = "stdout",
            Passed = true,
            Message = "Output contains expected text",
            ExpectedValue = "Success",
            ActualValue = "Success"
        };

        // Assert
        Assert.IsTrue(result.Passed);
        Assert.AreEqual("stdout", result.Type);
    }

    [TestMethod]
    public void AssertionResult_FailedScenario()
    {
        // Arrange & Act
        var result = new AssertionResult
        {
            Type = "exitCode",
            Passed = false,
            Message = "Exit code mismatch",
            ExpectedValue = "0",
            ActualValue = "1"
        };

        // Assert
        Assert.IsFalse(result.Passed);
        Assert.AreNotEqual(result.ExpectedValue, result.ActualValue);
    }
}

[TestClass]
public class ExtractionResultTests
{
    [TestMethod]
    public void Constructor_InitializesWithDefaults()
    {
        // Act
        var result = new ExtractionResult();

        // Assert
        Assert.IsNull(result.Name);
        Assert.IsNull(result.Value);
        Assert.IsFalse(result.Success);
        Assert.IsNull(result.Message);
    }

    [TestMethod]
    public void Properties_CanBeSet()
    {
        // Arrange
        var result = new ExtractionResult();

        // Act
        result.Name = "version";
        result.Value = "1.2.3";
        result.Success = true;
        result.Message = "Successfully extracted version";

        // Assert
        Assert.AreEqual("version", result.Name);
        Assert.AreEqual("1.2.3", result.Value);
        Assert.IsTrue(result.Success);
        Assert.AreEqual("Successfully extracted version", result.Message);
    }

    [TestMethod]
    public void ExtractionResult_SuccessfulScenario()
    {
        // Arrange & Act
        var result = new ExtractionResult
        {
            Name = "buildNumber",
            Value = "12345",
            Success = true,
            Message = "Build number extracted"
        };

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual("12345", result.Value);
    }

    [TestMethod]
    public void ExtractionResult_FailureScenario()
    {
        // Arrange & Act
        var result = new ExtractionResult
        {
            Name = "version",
            Value = null,
            Success = false,
            Message = "Pattern did not match"
        };

        // Assert
        Assert.IsFalse(result.Success);
        Assert.IsNull(result.Value);
    }
}
