#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YamlLauncher;
using YamlLauncher.Models;

namespace LauncherTests.YamlLauncher;

[TestClass]
public class StepExecutorTests
{
    private StepExecutor _executor = null!;

    [TestInitialize]
    public void Setup()
    {
        _executor = new StepExecutor(verbose: false);
    }

    [TestMethod]
    public async Task LaunchAsync_WithValidSimpleStep_Succeeds()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>
            {
                new StepConfig
                {
                    Name = "test",
                    Path = "cmd.exe",
                    Arguments = "/c \"echo Hello World\"",
                    ExpectedReturnCode = 0
                }
            }
        };

        var result = await _executor.LaunchAsync(config, 0);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Results!.Count);
        Assert.AreEqual(0, result.Results[0].ExitCode);
        Assert.IsTrue(result.Results[0].StdOut!.Contains("Hello World"));
    }

    [TestMethod]
    public async Task LaunchAsync_WithNonZeroExitCode_ReturnsFalse()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>
            {
                new StepConfig
                {
                    Name = "test",
                    Path = "cmd.exe",
                    Arguments = "/c \"exit 1\"",
                    ExpectedReturnCode = 0
                }
            }
        };

        var result = await _executor.LaunchAsync(config, 0);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(1, result.Results!.Count);
        Assert.AreEqual(1, result.Results[0].ExitCode);
    }

    [TestMethod]
    public async Task LaunchAsync_WithMatchingExpectedReturnCode_Succeeds()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>
            {
                new StepConfig
                {
                    Name = "test",
                    Path = "cmd.exe",
                    Arguments = "/c \"exit 5\"",
                    ExpectedReturnCode = 5
                }
            }
        };

        var result = await _executor.LaunchAsync(config, 0);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(5, result.Results![0].ExitCode);
    }

    [TestMethod]
    public async Task LaunchAsync_WithStdErr_CapturesError()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>
            {
                new StepConfig
                {
                    Name = "test",
                    Path = "cmd.exe",
                    Arguments = "/c \"cmd.exe /c (echo Error message 1>&2) && exit 0\"",
                    ExpectedReturnCode = 0
                }
            }
        };

        var result = await _executor.LaunchAsync(config, 0);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Results!.Count);
        Assert.IsTrue(result.Results[0].StdErr!.Length > 0);
    }

    [TestMethod]
    public async Task LaunchAsync_MeasuresExecutionTime()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>
            {
                new StepConfig
                {
                    Name = "test",
                    Path = "cmd.exe",
                    Arguments = "/c \"timeout /t 1 /nobreak > nul 2>&1 && echo Done\"",
                    ExpectedReturnCode = 0
                }
            }
        };

        var result = await _executor.LaunchAsync(config, 0);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.DurationMs > 0);
        Assert.IsTrue(result.Results![0].DurationMs > 0);
    }

    [TestMethod]
    public async Task LaunchAsync_WithNoSteps_Fails()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>()
        };

        try
        {
            await _executor.LaunchAsync(config, 0);
            Assert.Fail("Should have thrown ArgumentException");
        }
        catch (ArgumentException ex)
        {
            Assert.IsTrue(ex.Message.Contains("no steps") || ex.Message.Contains("out of range"));
        }
    }

    [TestMethod]
    public async Task LaunchAsync_WithStepIndexOutOfRange_Fails()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>
            {
                new StepConfig { Name = "step1", Path = "cmd.exe" }
            }
        };

        try
        {
            await _executor.LaunchAsync(config, 5);
            Assert.Fail("Should have thrown ArgumentException");
        }
        catch (ArgumentException ex)
        {
            Assert.IsTrue(ex.Message.Contains("out of range"));
        }
    }

    [TestMethod]
    public async Task LaunchAsync_WithNullConfig_Fails()
    {
        try
        {
            await _executor.LaunchAsync(null!, 0);
            Assert.Fail("Should have thrown ArgumentNullException");
        }
        catch (ArgumentNullException)
        {
            // Expected
        }
    }

    [TestMethod]
    public async Task LaunchAsync_WithEnvironmentVariables_PassesThemToProcess()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Variables = new Dictionary<string, string>
            {
                { "TEST_VAR", "TestValue123" }
            },
            Steps = new List<StepConfig>
            {
                new StepConfig
                {
                    Name = "test",
                    Path = "cmd.exe",
                    Arguments = "/c \"echo %TEST_VAR%\"",
                    ExpectedReturnCode = 0
                }
            }
        };

        var result = await _executor.LaunchAsync(config, 0);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Results![0].StdOut!.Contains("TestValue123"));
    }

    [TestMethod]
    public async Task LaunchAsync_WithWorkingDirectory_UsesIt()
    {
        var tempDir = Path.GetTempPath();
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>
            {
                new StepConfig
                {
                    Name = "test",
                    Path = "cmd.exe",
                    Arguments = "/c \"cd\"",
                    WorkingDirectory = tempDir,
                    ExpectedReturnCode = 0
                }
            }
        };

        var result = await _executor.LaunchAsync(config, 0);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public async Task LaunchAsync_WithInvalidWorkingDirectory_Fails()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>
            {
                new StepConfig
                {
                    Name = "test",
                    Path = "cmd.exe",
                    Arguments = "/c \"echo test\"",
                    WorkingDirectory = "/nonexistent/path/that/does/not/exist",
                    ExpectedReturnCode = 0
                }
            }
        };

        var result = await _executor.LaunchAsync(config, 0);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(-1, result.Results![0].ExitCode);
    }

    [TestMethod]
    public async Task LaunchAsync_WithNonExistentExecutable_Fails()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>
            {
                new StepConfig
                {
                    Name = "test",
                    Path = "/nonexistent/path/to/executable.exe",
                    ExpectedReturnCode = 0
                }
            }
        };

        var result = await _executor.LaunchAsync(config, 0);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(-1, result.Results![0].ExitCode);
    }

    [TestMethod]
    public async Task LaunchAsync_ExecutesCorrectStep_ByIndex()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>
            {
                new StepConfig
                {
                    Name = "step1",
                    Path = "cmd.exe",
                    Arguments = "/c \"echo Step1\"",
                    ExpectedReturnCode = 0
                },
                new StepConfig
                {
                    Name = "step2",
                    Path = "cmd.exe",
                    Arguments = "/c \"echo Step2\"",
                    ExpectedReturnCode = 0
                }
            }
        };

        var result = await _executor.LaunchAsync(config, 1);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
        Assert.AreEqual("step2", result.Results![0].StepName);
        Assert.IsTrue(result.Results[0].StdOut!.Contains("Step2"));
    }

    [TestMethod]
    public async Task LaunchAsync_ResultsHaveCorrectMetadata()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>
            {
                new StepConfig
                {
                    Name = "testStep",
                    Path = "cmd.exe",
                    Arguments = "/c \"echo test\"",
                    ExpectedReturnCode = 0
                }
            }
        };

        var result = await _executor.LaunchAsync(config, 0);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.StartTime);
        Assert.IsNotNull(result.EndTime);
        Assert.IsTrue(result.EndTime >= result.StartTime);
        Assert.AreEqual(1, result.Results!.Count);
        Assert.AreEqual("testStep", result.Results[0].StepName);
    }
}
