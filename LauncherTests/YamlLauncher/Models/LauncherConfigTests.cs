using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YamlLauncher.Models;

namespace LauncherTests.YamlLauncher.Models;

[TestClass]
public class LauncherConfigTests
{
    [TestMethod]
    public void Constructor_InitializesWithDefaults()
    {
        // Act
        var config = new LauncherConfig();

        // Assert
        Assert.IsNull(config.Version);
        Assert.IsNull(config.Description);
        Assert.IsNull(config.Execution);
        Assert.IsNull(config.Variables);
        Assert.IsNull(config.Steps);
    }

    [TestMethod]
    public void Properties_CanBeSet()
    {
        // Arrange
        var config = new LauncherConfig();

        // Act
        config.Version = "1.0";
        config.Description = "Test configuration";
        config.Execution = new ExecutionSettings { Mode = ExecutionMode.Parallel };
        config.Variables = new Dictionary<string, string> { { "key", "value" } };
        config.Steps = new List<StepConfig> { new StepConfig { Path = "cmd" } };

        // Assert
        Assert.AreEqual("1.0", config.Version);
        Assert.AreEqual("Test configuration", config.Description);
        Assert.IsNotNull(config.Execution);
        Assert.IsNotNull(config.Variables);
        Assert.IsNotNull(config.Steps);
    }

    [TestMethod]
    public void Tasks_Alias_ReferencesSteps()
    {
        // Arrange
        var config = new LauncherConfig();
        var steps = new List<StepConfig> { new StepConfig { Path = "cmd" } };

        // Act
        config.Steps = steps;

        // Assert
        Assert.AreSame(config.Tasks, config.Steps);
        Assert.AreEqual(1, config.Tasks!.Count);
    }

    [TestMethod]
    public void Tasks_Alias_CanSetSteps()
    {
        // Arrange
        var config = new LauncherConfig();
        var tasks = new List<StepConfig> { new StepConfig { Path = "task1" } };

        // Act
        config.Tasks = tasks;

        // Assert
        Assert.AreSame(config.Steps, tasks);
        Assert.AreEqual(1, config.Steps!.Count);
        Assert.AreEqual("task1", config.Steps[0].Path);
    }

    [TestMethod]
    public void Apps_Alias_ReferencesSteps()
    {
        // Arrange
        var config = new LauncherConfig();
        var steps = new List<StepConfig> { new StepConfig { Path = "app" } };

        // Act
        config.Steps = steps;

        // Assert
        Assert.AreSame(config.Apps, config.Steps);
        Assert.AreEqual(1, config.Apps!.Count);
    }

    [TestMethod]
    public void Apps_Alias_CanSetSteps()
    {
        // Arrange
        var config = new LauncherConfig();
        var apps = new List<StepConfig> { new StepConfig { Path = "app1" } };

        // Act
        config.Apps = apps;

        // Assert
        Assert.AreSame(config.Steps, apps);
        Assert.AreEqual(1, config.Steps!.Count);
        Assert.AreEqual("app1", config.Steps[0].Path);
    }

    [TestMethod]
    public void Steps_Tasks_Apps_AreEquivalent()
    {
        // Arrange
        var config = new LauncherConfig();
        var step1 = new StepConfig { Name = "step1", Path = "cmd1" };
        var step2 = new StepConfig { Name = "step2", Path = "cmd2" };

        // Act
        config.Steps = new List<StepConfig> { step1, step2 };

        // Assert
        Assert.AreEqual(config.Steps.Count, config.Tasks!.Count);
        Assert.AreEqual(config.Steps.Count, config.Apps!.Count);
        Assert.AreSame(config.Steps[0], config.Tasks[0]);
        Assert.AreSame(config.Steps[1], config.Apps[1]);
    }

    [TestMethod]
    public void LauncherConfig_WithAllProperties()
    {
        // Arrange & Act
        var config = new LauncherConfig
        {
            Version = "1.0",
            Description = "Integration test",
            Execution = new ExecutionSettings
            {
                Mode = ExecutionMode.Sequential,
                Verbose = true
            },
            Variables = new Dictionary<string, string>
            {
                { "ENV", "test" },
                { "DEBUG", "true" }
            },
            Steps = new List<StepConfig>
            {
                new StepConfig { Path = "cmd1", Name = "step1" },
                new StepConfig { Path = "cmd2", Name = "step2" }
            }
        };

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual("1.0", config.Version);
        Assert.AreEqual(2, config.Steps.Count);
        Assert.AreEqual(2, config.Variables.Count);
        Assert.AreEqual(ExecutionMode.Sequential, config.Execution!.Mode);
    }

    [TestMethod]
    public void Variables_CanContainMultipleEntries()
    {
        // Arrange & Act
        var config = new LauncherConfig
        {
            Variables = new Dictionary<string, string>
            {
                { "DEBUG", "true" },
                { "LOG_LEVEL", "info" },
                { "CONFIG_PATH", "/etc/config" }
            }
        };

        // Assert
        Assert.AreEqual(3, config.Variables!.Count);
        Assert.AreEqual("true", config.Variables["DEBUG"]);
        Assert.AreEqual("info", config.Variables["LOG_LEVEL"]);
    }

    [TestMethod]
    public void Execution_DefaultsToNull()
    {
        // Arrange & Act
        var config = new LauncherConfig();

        // Assert
        Assert.IsNull(config.Execution);
    }

    [TestMethod]
    public void ComplexScenario_WithDependenciesAndAssertions()
    {
        // Arrange & Act
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>
            {
                new StepConfig
                {
                    Name = "restore",
                    Path = "dotnet",
                    Arguments = "restore",
                    Assertions = new List<Assertion>
                    {
                        new Assertion { Type = "exitCode", Value = "0" }
                    }
                },
                new StepConfig
                {
                    Name = "build",
                    Path = "dotnet",
                    Arguments = "build",
                    Dependencies = new List<string> { "restore" },
                    Assertions = new List<Assertion>
                    {
                        new Assertion { Type = "exitCode", Value = "0" },
                        new Assertion { Type = "stdout", Pattern = "Build succeeded" }
                    },
                    ExtractVariables = new List<VariableExtraction>
                    {
                        new VariableExtraction { Name = "buildOutput", Pattern = @"Output: (.+)" }
                    }
                }
            }
        };

        // Assert
        Assert.AreEqual(2, config.Steps.Count);
        Assert.AreEqual("build", config.Steps[1].Name);
        Assert.AreEqual(1, config.Steps[1].Dependencies!.Count);
        Assert.AreEqual(2, config.Steps[1].Assertions!.Count);
        Assert.AreEqual(1, config.Steps[1].ExtractVariables!.Count);
    }
}
