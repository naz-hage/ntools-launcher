using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YamlLauncher.Models;

namespace LauncherTests.YamlLauncher.Models;

[TestClass]
public class AssertionTests
{
    [TestMethod]
    public void Constructor_InitializesWithDefaults()
    {
        // Act
        var assertion = new Assertion();

        // Assert
        Assert.IsNull(assertion.Type);
        Assert.IsNull(assertion.Value);
        Assert.IsNull(assertion.Pattern);
        Assert.IsNull(assertion.JsonPath);
        Assert.IsNull(assertion.ExpectedValue);
        Assert.IsFalse(assertion.CaseInsensitive);
        Assert.IsNull(assertion.Description);
    }

    [TestMethod]
    public void Properties_CanBeSet()
    {
        // Arrange
        var assertion = new Assertion();

        // Act
        assertion.Type = "exitCode";
        assertion.Value = "0";
        assertion.Pattern = @"\d+";
        assertion.JsonPath = "$.result";
        assertion.ExpectedValue = "success";
        assertion.CaseInsensitive = true;
        assertion.Description = "Check exit code is zero";

        // Assert
        Assert.AreEqual("exitCode", assertion.Type);
        Assert.AreEqual("0", assertion.Value);
        Assert.AreEqual(@"\d+", assertion.Pattern);
        Assert.AreEqual("$.result", assertion.JsonPath);
        Assert.AreEqual("success", assertion.ExpectedValue);
        Assert.IsTrue(assertion.CaseInsensitive);
        Assert.AreEqual("Check exit code is zero", assertion.Description);
    }

    [TestMethod]
    public void Assertion_WithAllProperties()
    {
        // Arrange & Act
        var assertion = new Assertion
        {
            Type = "stdout",
            Value = "Completed",
            Pattern = @"[Cc]ompleted",
            JsonPath = "$.status",
            ExpectedValue = "Completed",
            CaseInsensitive = false,
            Description = "Verify completion message in output"
        };

        // Assert
        Assert.IsNotNull(assertion);
        Assert.AreEqual("stdout", assertion.Type);
        Assert.AreEqual("Completed", assertion.Value);
        Assert.IsFalse(assertion.CaseInsensitive);
    }
}
