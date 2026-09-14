#nullable enable

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YamlLauncher;
using YamlLauncher.Models;

namespace LauncherTests.YamlLauncher;

[TestClass]
public class YamlLauncherConfigLoaderTests
{
    private YamlLauncherConfigLoader _loader = null!;

    [TestInitialize]
    public void Setup()
    {
        _loader = new YamlLauncherConfigLoader();
    }

    [TestMethod]
    public async Task LoadFromString_WithValidSimpleConfig_ReturnsConfig()
    {
        var yaml = @"version: '1.0'
description: 'Simple test'
steps:
  - name: 'step1'
    path: '/usr/bin/echo'
";
        var config = await _loader.LoadFromStringAsync(yaml);
        Assert.IsNotNull(config);
        Assert.AreEqual("1.0", config.Version);
        Assert.AreEqual(1, config.Steps!.Count);
    }

    [TestMethod]
    public async Task LoadFromString_WithTasksAlias_Deserializes()
    {
        var yaml = @"version: '1.0'
tasks:
  - name: 'task1'
    path: '/usr/bin/cmd'
";
        var config = await _loader.LoadFromStringAsync(yaml);
        Assert.IsNotNull(config.Steps);
        Assert.AreEqual(1, config.Steps.Count);
    }

    [TestMethod]
    public async Task LoadFromString_MissingVersion_Fails()
    {
        var yaml = @"steps:
  - name: 'step1'
    path: '/usr/bin/cmd'
";
        try
        {
            await _loader.LoadFromStringAsync(yaml);
            Assert.Fail("Should have thrown LauncherConfigException");
        }
        catch (LauncherConfigException)
        {
            // Expected
        }
    }

    [TestMethod]
    public async Task LoadFromString_NoSteps_Fails()
    {
        var yaml = @"version: '1.0'";
        try
        {
            await _loader.LoadFromStringAsync(yaml);
            Assert.Fail("Should have thrown LauncherConfigException");
        }
        catch (LauncherConfigException)
        {
            // Expected
        }
    }

    [TestMethod]
    public async Task LoadFromString_StepMissingPath_Fails()
    {
        var yaml = @"version: '1.0'
steps:
  - name: 'step1'
";
        try
        {
            await _loader.LoadFromStringAsync(yaml);
            Assert.Fail("Should have thrown LauncherConfigException");
        }
        catch (LauncherConfigException)
        {
            // Expected
        }
    }

    [TestMethod]
    public async Task LoadFromString_NullContent_Fails()
    {
        try
        {
            await _loader.LoadFromStringAsync(null!);
            Assert.Fail("Should have thrown ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public async Task LoadFromFile_NonexistentFile_Fails()
    {
        try
        {
            await _loader.LoadFromFileAsync("/nonexistent/path/config.yaml");
            Assert.Fail("Should have thrown FileNotFoundException");
        }
        catch (FileNotFoundException)
        {
            // Expected
        }
    }

    [TestMethod]
    public async Task LoadFromFile_WithValidFile_Loads()
    {
        var tempFile = Path.GetTempFileName() + ".yaml";
        try
        {
            File.WriteAllText(tempFile, @"version: '1.0'
description: 'Test'
steps:
  - name: 'step1'
    path: '/usr/bin/cmd'
");
            var config = await _loader.LoadFromFileAsync(tempFile);
            Assert.IsNotNull(config);
            Assert.AreEqual("1.0", config.Version);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [TestMethod]
    public async Task LoadFromStream_WithValidStream_Loads()
    {
        var yaml = @"version: '1.0'
steps:
  - name: 'step1'
    path: '/usr/bin/cmd'
";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));
        var config = await _loader.LoadFromStreamAsync(stream);
        Assert.IsNotNull(config);
    }

    [TestMethod]
    public async Task LoadFromStream_NullStream_Fails()
    {
        try
        {
            await _loader.LoadFromStreamAsync(null!);
            Assert.Fail("Should have thrown ArgumentNullException");
        }
        catch (ArgumentNullException)
        {
            // Expected
        }
    }

    [TestMethod]
    public async Task LoadFromString_DanglingDependency_Fails()
    {
        var yaml = @"version: '1.0'
steps:
  - name: 'step1'
    path: '/usr/bin/cmd'
    dependencies:
      - nonexistent
";
        try
        {
            await _loader.LoadFromStringAsync(yaml);
            Assert.Fail("Should have thrown LauncherConfigException");
        }
        catch (LauncherConfigException)
        {
            // Expected
        }
    }
}
