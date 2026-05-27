using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YamlLauncher.Models;

namespace LauncherTests.YamlLauncher.Models;

[TestClass]
public class VariableExtractionTests
{
    [TestMethod]
    public void Constructor_InitializesWithDefaults()
    {
        // Act
        var extraction = new VariableExtraction();

        // Assert
        Assert.IsNull(extraction.Name);
        Assert.IsNull(extraction.Pattern);
        Assert.AreEqual(0, extraction.GroupIndex);
        Assert.IsFalse(extraction.CaseInsensitive);
        Assert.IsNull(extraction.Description);
    }

    [TestMethod]
    public void Properties_CanBeSet()
    {
        // Arrange
        var extraction = new VariableExtraction();

        // Act
        extraction.Name = "version";
        extraction.Pattern = @"v(\d+\.\d+\.\d+)";
        extraction.GroupIndex = 1;
        extraction.CaseInsensitive = true;
        extraction.Description = "Extract semantic version";

        // Assert
        Assert.AreEqual("version", extraction.Name);
        Assert.AreEqual(@"v(\d+\.\d+\.\d+)", extraction.Pattern);
        Assert.AreEqual(1, extraction.GroupIndex);
        Assert.IsTrue(extraction.CaseInsensitive);
        Assert.AreEqual("Extract semantic version", extraction.Description);
    }

    [TestMethod]
    public void GroupIndex_DefaultsToZero()
    {
        // Arrange
        var extraction = new VariableExtraction
        {
            Name = "output",
            Pattern = @".*"
        };

        // Assert
        Assert.AreEqual(0, extraction.GroupIndex);
    }

    [TestMethod]
    public void Extraction_WithAllProperties()
    {
        // Arrange & Act
        var extraction = new VariableExtraction
        {
            Name = "buildNumber",
            Pattern = @"Build: (\d+)",
            GroupIndex = 1,
            CaseInsensitive = false,
            Description = "Extract build number from output"
        };

        // Assert
        Assert.IsNotNull(extraction);
        Assert.AreEqual("buildNumber", extraction.Name);
        Assert.AreEqual(1, extraction.GroupIndex);
        Assert.IsFalse(extraction.CaseInsensitive);
    }
}
