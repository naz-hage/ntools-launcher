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
        [TestMethod()]
        public void GetFullPathOfFileTest()
        {
            // Arrange
            var file = "git.exe";
            var expected = @"C:\Program Files\git\cmd\git.exe";

            // Act
            var actual = ShellUtility.GetFullPathOfFile(file);

            // Assert
            // compare expected and actual strings not case sensitive
            Assert.AreEqual(expected, actual, true);
        }
    }
}