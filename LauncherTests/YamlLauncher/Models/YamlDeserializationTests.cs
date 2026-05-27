using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YamlLauncher.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LauncherTests.YamlLauncher.Models;

[TestClass]
public class YamlDeserializationTests
{
    [TestMethod]
    public void LauncherConfig_CanDeserializeFromYaml()
    {
        // Arrange
        var yaml = @"
version: '1.0'
description: 'Test configuration'
execution:
  mode: Sequential
  verbose: true
  stopOnFirstError: false
  maxConcurrency: 4
  timeout: 300
variables:
  DEBUG: 'true'
  ENV: 'test'
steps:
  - name: 'step1'
    path: '/usr/bin/cmd'
    arguments: 'arg1'
    continueOnError: false
    expectedReturnCode: 0
  - name: 'step2'
    path: '/usr/bin/cmd2'
    arguments: 'arg2'
    dependencies:
      - 'step1'
";
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        // Act
        var config = deserializer.Deserialize<LauncherConfig>(yaml);

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual("1.0", config.Version);
        Assert.AreEqual("Test configuration", config.Description);
        Assert.IsNotNull(config.Execution);
        Assert.AreEqual(ExecutionMode.Sequential, config.Execution.Mode);
        Assert.IsTrue(config.Execution.Verbose);
        Assert.AreEqual(2, config.Steps!.Count);
        Assert.AreEqual("step1", config.Steps[0].Name);
        Assert.AreEqual("step2", config.Steps[1].Name);
    }

    [TestMethod]
    public void LauncherConfig_WithTasksAlias_DeserializesCorrectly()
    {
        // Arrange - using "tasks:" instead of "steps:"
        var yaml = @"
version: '1.0'
tasks:
  - name: 'task1'
    path: '/usr/bin/cmd'
";
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        // Act
        var config = deserializer.Deserialize<LauncherConfig>(yaml);

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual(1, config.Steps!.Count);
        Assert.AreEqual("task1", config.Steps[0].Name);
        Assert.AreEqual(config.Tasks, config.Steps);
    }

    [TestMethod]
    public void LauncherConfig_WithAppsAlias_DeserializesCorrectly()
    {
        // Arrange - using "apps:" instead of "steps:"
        var yaml = @"
version: '1.0'
apps:
  - name: 'app1'
    path: '/usr/bin/app'
";
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        // Act
        var config = deserializer.Deserialize<LauncherConfig>(yaml);

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual(1, config.Steps!.Count);
        Assert.AreEqual("app1", config.Steps[0].Name);
        Assert.AreEqual(config.Apps, config.Steps);
    }

    [TestMethod]
    public void StepConfig_WithAssertionsAndExtractVariables_DeserializesCorrectly()
    {
        // Arrange
        var yaml = @"
name: 'test-step'
path: '/usr/bin/test'
arguments: '-v'
continueOnError: false
expectedReturnCode: 0
assertions:
  - type: 'exitCode'
    value: '0'
  - type: 'stdout'
    pattern: 'Success'
extractVariables:
  - name: 'version'
    pattern: 'v(\d+)'
    groupIndex: 1
";
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        // Act
        var step = deserializer.Deserialize<StepConfig>(yaml);

        // Assert
        Assert.IsNotNull(step);
        Assert.AreEqual("test-step", step.Name);
        Assert.AreEqual(2, step.Assertions!.Count);
        Assert.AreEqual(1, step.ExtractVariables!.Count);
        Assert.AreEqual("version", step.ExtractVariables[0].Name);
    }

    [TestMethod]
    public void ComplexYamlStructure_DeserializesSuccessfully()
    {
        // Arrange - complex real-world scenario
        var yaml = @"
version: '2.0'
description: 'Complex deployment workflow'
execution:
  mode: Parallel
  verbose: true
  stopOnFirstError: true
  maxConcurrency: 8
  timeout: 600
variables:
  DEPLOY_ENV: 'production'
  LOG_LEVEL: 'info'
steps:
  - name: 'validate'
    path: '/scripts/validate.sh'
    assertions:
      - type: 'exitCode'
        expectedValue: '0'
  - name: 'build'
    path: '/scripts/build.sh'
    dependencies:
      - 'validate'
    extractVariables:
      - name: 'buildId'
        pattern: 'Build ID: (\w+)'
        groupIndex: 1
  - name: 'deploy'
    path: '/scripts/deploy.sh'
    arguments: '--env production'
    dependencies:
      - 'build'
    continueOnError: false
";
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        // Act
        var config = deserializer.Deserialize<LauncherConfig>(yaml);

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual("2.0", config.Version);
        Assert.AreEqual(ExecutionMode.Parallel, config.Execution!.Mode);
        Assert.AreEqual(3, config.Steps!.Count);
        Assert.AreEqual("validate", config.Steps[0].Name);
        Assert.AreEqual("build", config.Steps[1].Name);
        Assert.AreEqual("deploy", config.Steps[2].Name);
        Assert.AreEqual(1, config.Steps[1].Dependencies!.Count);
        Assert.AreEqual(1, config.Steps[1].ExtractVariables!.Count);
    }

    [TestMethod]
    public void LaunchResult_CanSerializeToYaml()
    {
        // Arrange
        var result = new LaunchResult
        {
            Success = true,
            Results = new List<ExecutionResult>
            {
                new ExecutionResult
                {
                    Name = "step1",
                    Success = true,
                    ExitCode = 0,
                    StandardOutput = "Success"
                }
            },
            ExtractedVariables = new Dictionary<string, string>
            {
                { "version", "1.0.0" }
            }
        };
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        // Act
        var yaml = serializer.Serialize(result);

        // Assert
        Assert.IsNotNull(yaml);
        Assert.IsTrue(yaml.Contains("success"));
        Assert.IsTrue(yaml.Contains("step1"));
        Assert.IsTrue(yaml.Contains("1.0.0"));
    }

    [TestMethod]
    public void BackwardCompatibility_TasksAndAppsAliasesWork()
    {
        // Arrange - verify that old configs using "tasks:" still work
        var yamlWithTasks = @"
version: '1.0'
tasks:
  - name: 'legacy-task'
    path: '/usr/bin/cmd'
";
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        // Act
        var config = deserializer.Deserialize<LauncherConfig>(yamlWithTasks);

        // Assert
        Assert.IsNotNull(config);
        Assert.IsNotNull(config.Steps);
        Assert.IsNotNull(config.Tasks);
        Assert.IsNotNull(config.Apps);
        Assert.AreSame(config.Steps, config.Tasks);
        Assert.AreSame(config.Steps, config.Apps);
        Assert.AreEqual("legacy-task", config.Steps[0].Name);
    }
}
