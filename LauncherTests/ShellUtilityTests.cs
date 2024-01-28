using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ntools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntools.Tests
{
    [TestClass()]
    public class ShellUtilityTests
    {
        [TestMethod,Ignore]
        public void GetFullPathOfFileTest()
        {
            // Arrange
            var file = "git.exe";
            var expected = @"C:\Program Files\git\cmd\git.exe";
            // on GitHub Actions, git.exe is 
            var githubActions = Environment.GetEnvironmentVariable("GITHUB_ACTIONS", EnvironmentVariableTarget.User);
            if (githubActions != null && githubActions.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                expected = @"C:\Program Files\Git\bin\git.exe"; //C:\Program Files\Git\bin\git.exe
            }
            
            // Act
            var actual = ShellUtility.GetFullPathOfFile(file);

            // Assert
            // compare expected and actual strings not case sensitive
            Assert.AreEqual(expected, actual, true);
        }
    }
}