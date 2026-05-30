using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YamlLauncher.Models;

namespace LauncherTests.YamlLauncher.Models;

[TestClass]
public class ExecutionSettingsTests
{
    [TestMethod]
    public void Constructor_InitializesWithDefaults()
    {
        // Act
        var settings = new ExecutionSettings();

        // Assert
        Assert.AreEqual(ExecutionMode.Sequential, settings.Mode);
        Assert.IsFalse(settings.Verbose);
        Assert.IsFalse(settings.StopOnFirstError);
        Assert.AreEqual(Environment.ProcessorCount, settings.MaxConcurrency);
        Assert.AreEqual(0, settings.Timeout);
    }

    [TestMethod]
    public void Mode_CanBeSetToParallel()
    {
        // Arrange
        var settings = new ExecutionSettings();

        // Act
        settings.Mode = ExecutionMode.Parallel;

        // Assert
        Assert.AreEqual(ExecutionMode.Parallel, settings.Mode);
    }

    [TestMethod]
    public void Mode_DefaultIsSequential()
    {
        // Arrange & Act
        var settings = new ExecutionSettings();

        // Assert
        Assert.AreEqual(ExecutionMode.Sequential, settings.Mode);
    }

    [TestMethod]
    public void Properties_CanBeSet()
    {
        // Arrange
        var settings = new ExecutionSettings();

        // Act
        settings.Mode = ExecutionMode.Parallel;
        settings.Verbose = true;
        settings.StopOnFirstError = true;
        settings.MaxConcurrency = 4;
        settings.Timeout = 300;

        // Assert
        Assert.AreEqual(ExecutionMode.Parallel, settings.Mode);
        Assert.IsTrue(settings.Verbose);
        Assert.IsTrue(settings.StopOnFirstError);
        Assert.AreEqual(4, settings.MaxConcurrency);
        Assert.AreEqual(300, settings.Timeout);
    }

    [TestMethod]
    public void MaxConcurrency_DefaultsToProcessorCount()
    {
        // Arrange & Act
        var settings = new ExecutionSettings();

        // Assert
        Assert.AreEqual(Environment.ProcessorCount, settings.MaxConcurrency);
    }

    [TestMethod]
    public void Timeout_DefaultsToZero()
    {
        // Arrange & Act
        var settings = new ExecutionSettings();

        // Assert
        Assert.AreEqual(0, settings.Timeout);
    }

    [TestMethod]
    public void ExecutionSettings_WithAllProperties()
    {
        // Arrange & Act
        var settings = new ExecutionSettings
        {
            Mode = ExecutionMode.Parallel,
            Verbose = true,
            StopOnFirstError = false,
            MaxConcurrency = 8,
            Timeout = 600
        };

        // Assert
        Assert.IsNotNull(settings);
        Assert.AreEqual(ExecutionMode.Parallel, settings.Mode);
        Assert.IsTrue(settings.Verbose);
        Assert.AreEqual(8, settings.MaxConcurrency);
        Assert.AreEqual(600, settings.Timeout);
    }
}

[TestClass]
public class ExecutionModeTests
{
    [TestMethod]
    public void ExecutionMode_HasSequentialValue()
    {
        // Assert
        Assert.AreEqual(0, (int)ExecutionMode.Sequential);
    }

    [TestMethod]
    public void ExecutionMode_HasParallelValue()
    {
        // Assert
        Assert.AreEqual(1, (int)ExecutionMode.Parallel);
    }
}
