#nullable enable

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YamlLauncher;
using YamlLauncher.Models;

namespace LauncherTests.YamlLauncher;

[TestClass]
public class LauncherConfigValidatorTests
{
    private LauncherConfigValidator _validator = null!;

    [TestInitialize]
    public void Setup()
    {
        _validator = new LauncherConfigValidator();
    }

    [TestMethod]
    public void Validate_ValidConfig_Succeeds()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>
            {
                new StepConfig { Name = "step1", Path = "/usr/bin/cmd" }
            }
        };

        _validator.Validate(config);
    }

    [TestMethod]
    public void Validate_NullConfig_Fails()
    {
        try
        {
            _validator.Validate(null!);
            Assert.Fail("Should have thrown LauncherConfigException");
        }
        catch (LauncherConfigException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void Validate_MissingVersion_Fails()
    {
        var config = new LauncherConfig
        {
            Steps = new List<StepConfig>
            {
                new StepConfig { Name = "step1", Path = "/usr/bin/cmd" }
            }
        };

        try
        {
            _validator.Validate(config);
            Assert.Fail("Should have thrown LauncherConfigException");
        }
        catch (LauncherConfigException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void Validate_InvalidVersionFormat_Fails()
    {
        var config = new LauncherConfig
        {
            Version = "invalid",
            Steps = new List<StepConfig>
            {
                new StepConfig { Name = "step1", Path = "/usr/bin/cmd" }
            }
        };

        try
        {
            _validator.Validate(config);
            Assert.Fail("Should have thrown LauncherConfigException");
        }
        catch (LauncherConfigException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void Validate_NoSteps_Fails()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>()
        };

        try
        {
            _validator.Validate(config);
            Assert.Fail("Should have thrown LauncherConfigException");
        }
        catch (LauncherConfigException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void Validate_StepMissingPath_Fails()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>
            {
                new StepConfig { Name = "step1" }
            }
        };

        try
        {
            _validator.Validate(config);
            Assert.Fail("Should have thrown LauncherConfigException");
        }
        catch (LauncherConfigException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void Validate_InvalidExpectedReturnCode_Fails()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>
            {
                new StepConfig { Name = "step1", Path = "/usr/bin/cmd", ExpectedReturnCode = -1 }
            }
        };

        try
        {
            _validator.Validate(config);
            Assert.Fail("Should have thrown LauncherConfigException");
        }
        catch (LauncherConfigException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void Validate_DanglingDependency_Fails()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>
            {
                new StepConfig { Name = "step1", Path = "/usr/bin/cmd" },
                new StepConfig 
                { 
                    Name = "step2", 
                    Path = "/usr/bin/cmd2",
                    Dependencies = new List<string> { "nonexistent" }
                }
            }
        };

        try
        {
            _validator.Validate(config);
            Assert.Fail("Should have thrown LauncherConfigException");
        }
        catch (LauncherConfigException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void Validate_ValidDependencies_Succeeds()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Steps = new List<StepConfig>
            {
                new StepConfig { Name = "step1", Path = "/usr/bin/cmd1" },
                new StepConfig 
                { 
                    Name = "step2", 
                    Path = "/usr/bin/cmd2",
                    Dependencies = new List<string> { "step1" }
                }
            }
        };

        _validator.Validate(config);
    }

    [TestMethod]
    public void Validate_ComplexValidConfig_Succeeds()
    {
        var config = new LauncherConfig
        {
            Version = "1.0",
            Description = "Complex",
            Execution = new ExecutionSettings { Mode = ExecutionMode.Parallel },
            Variables = new Dictionary<string, string> { { "ENV", "test" } },
            Steps = new List<StepConfig>
            {
                new StepConfig { Name = "setup", Path = "/usr/bin/setup.sh" },
                new StepConfig 
                { 
                    Name = "build", 
                    Path = "/usr/bin/make",
                    Dependencies = new List<string> { "setup" }
                }
            }
        };

        _validator.Validate(config);
    }
}
