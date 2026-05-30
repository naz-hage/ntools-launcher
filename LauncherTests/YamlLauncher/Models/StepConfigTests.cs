using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YamlLauncher.Models;

namespace LauncherTests.YamlLauncher.Models;

[TestClass]
public class StepConfigTests
{
    [TestMethod]
    public void Constructor_InitializesWithDefaults()
    {
        // Act
        var step = new StepConfig();

        // Assert
        Assert.IsNull(step.Path);
        Assert.IsNull(step.Arguments);
        Assert.IsNull(step.Name);
        Assert.IsNull(step.Dependencies);
        Assert.IsFalse(step.ContinueOnError);
        Assert.AreEqual(0, step.ExpectedReturnCode);
        Assert.IsNull(step.Assertions);
        Assert.IsNull(step.ExtractVariables);
    }

    [TestMethod]
    public void Properties_CanBeSet()
    {
        // Arrange
        var step = new StepConfig();

        // Act
        step.Path = "/usr/bin/dotnet";
        step.Arguments = "build";
        step.Name = "build-step";
        step.Dependencies = new List<string> { "restore" };
        step.ContinueOnError = true;
        step.ExpectedReturnCode = 0;

        // Assert
        Assert.AreEqual("/usr/bin/dotnet", step.Path);
        Assert.AreEqual("build", step.Arguments);
        Assert.AreEqual("build-step", step.Name);
        Assert.IsNotNull(step.Dependencies);
        Assert.AreEqual(1, step.Dependencies.Count);
        Assert.AreEqual("restore", step.Dependencies[0]);
        Assert.IsTrue(step.ContinueOnError);
        Assert.AreEqual(0, step.ExpectedReturnCode);
    }

    [TestMethod]
    public void ExpectedReturnCode_DefaultsToZero()
    {
        // Arrange & Act
        var step = new StepConfig();

        // Assert
        Assert.AreEqual(0, step.ExpectedReturnCode);
    }

    [TestMethod]
    public void Dependencies_CanContainMultipleItems()
    {
        // Arrange & Act
        var step = new StepConfig
        {
            Name = "test",
            Dependencies = new List<string> { "build", "restore", "compile" }
        };

        // Assert
        Assert.AreEqual(3, step.Dependencies.Count);
        Assert.IsTrue(step.Dependencies.Contains("build"));
        Assert.IsTrue(step.Dependencies.Contains("restore"));
        Assert.IsTrue(step.Dependencies.Contains("compile"));
    }

    [TestMethod]
    public void Assertions_CanBeAdded()
    {
        // Arrange & Act
        var step = new StepConfig
        {
            Path = "dotnet",
            Assertions = new List<Assertion>
            {
                new Assertion { Type = "exitCode", Value = "0" },
                new Assertion { Type = "stdout", Pattern = "Success" }
            }
        };

        // Assert
        Assert.IsNotNull(step.Assertions);
        Assert.AreEqual(2, step.Assertions.Count);
    }

    [TestMethod]
    public void ExtractVariables_CanBeAdded()
    {
        // Arrange & Act
        var step = new StepConfig
        {
            Path = "command",
            ExtractVariables = new List<VariableExtraction>
            {
                new VariableExtraction { Name = "version", Pattern = @"v(\d+)" }
            }
        };

        // Assert
        Assert.IsNotNull(step.ExtractVariables);
        Assert.AreEqual(1, step.ExtractVariables.Count);
        Assert.AreEqual("version", step.ExtractVariables[0].Name);
    }

    [TestMethod]
    public void StepConfig_ComplexScenario()
    {
        // Arrange & Act
        var step = new StepConfig
        {
            Path = "dotnet",
            Arguments = "build -c Release",
            Name = "build",
            Dependencies = new List<string> { "restore" },
            ContinueOnError = false,
            ExpectedReturnCode = 0,
            Assertions = new List<Assertion>
            {
                new Assertion { Type = "exitCode", Value = "0" }
            },
            ExtractVariables = new List<VariableExtraction>
            {
                new VariableExtraction { Name = "buildDir", Pattern = @"Output: (.+)" }
            }
        };

        // Assert
        Assert.IsNotNull(step);
        Assert.AreEqual("dotnet", step.Path);
        Assert.AreEqual("build -c Release", step.Arguments);
        Assert.AreEqual("build", step.Name);
        Assert.AreEqual(1, step.Dependencies.Count);
        Assert.AreEqual(1, step.Assertions.Count);
        Assert.AreEqual(1, step.ExtractVariables.Count);
    }

    [TestMethod]
    public void ContinueOnError_DefaultsFalse()
    {
        // Arrange & Act
        var step = new StepConfig();

        // Assert
        Assert.IsFalse(step.ContinueOnError);
    }
}
