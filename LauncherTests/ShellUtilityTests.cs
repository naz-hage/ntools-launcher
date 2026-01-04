using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Ntools.Tests
{
    [TestClass()]
    [DoNotParallelize]
    public class ShellUtilityTests
    {
        [TestMethod]
        public void GetFullPathOfFileTest()
        {
            // Arrange
            var file = "xcopy.exe";
            var expected = @"C:\Windows\System32\xcopy.exe";
            
            // Act
            var actual = ShellUtility.GetFullPathOfFile(file);
            if (!string.IsNullOrEmpty(actual))
            {
                Console.WriteLine($"Full path of {file}: {actual}");
            }

            // Assert
            // compare expected and actual strings not case sensitive
            Assert.AreEqual(expected, actual, true);
        }

        [TestMethod]
        public void GetFullPathOfFileEmptyTest()
        {
            // Arrange for empty file
            var file = string.Empty;
            var expected = "";

            // Act
            var actual = ShellUtility.GetFullPathOfFile(file);
            if (!string.IsNullOrEmpty(actual))
            {
                Console.WriteLine($"Full path of {file}: {actual}");
            }

            // Assert
            // compare expected and actual strings not case sensitive
            Assert.AreEqual(expected, actual, true);

        }

        [TestMethod]
        public void GetFullPathOfFileNullTest()
        {
            // Arrange for null file
            string file = null;
            var expected = "";

            // Act
            var actual = ShellUtility.GetFullPathOfFile(file);
            if (!string.IsNullOrEmpty(actual))
            {
                Console.WriteLine($"Full path of {file}: {actual}");
            }

            // Assert
            // compare expected and actual strings not case sensitive
            Assert.AreEqual(expected, actual, true);


        }

        [TestMethod]
        public void GetFullPathOfFileUnknownTest()
        {

            // Arrange for file that is unknown
            var file = Guid.NewGuid().ToString();
            var expected = "";

            // Act
            var actual = ShellUtility.GetFullPathOfFile(file);
            if (!string.IsNullOrEmpty(actual))
            {
                Console.WriteLine($"Full path of {file}: {actual}");
            }

            // Assert
            // compare expected and actual strings not case sensitive
            Assert.AreEqual(expected, actual, true);

            // Assert
            // compare expected and actual strings not case sensitive
            Assert.AreEqual(expected, actual, true);


        }

        [TestMethod]
        public void GetFullPathOfFileInvalidTest()
        {
            // Arrange for file with invalid characters
            var file = ":this";
            var expected = "";

            // Act
            var actual = ShellUtility.GetFullPathOfFile(file);
            if (!string.IsNullOrEmpty(actual))
            {
                Console.WriteLine($"Full path of {file}: {actual}");
            }

            // Assert
            // compare expected and actual strings not case sensitive
            Assert.AreEqual(expected, actual, true);
        }   
    }
}