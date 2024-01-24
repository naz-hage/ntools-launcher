using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ntools;
using System;
using System.Diagnostics;

namespace Launcher.Tests
{
    [TestClass()]
    public class ProcessExtensionsTests
    {
        [TestMethod()]
        public void LockVerifyTest()
        {
            // Arrange
            var process = new Process();
            process.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System);
            process.StartInfo.FileName = $"robocopy.exe";  // must be path rooted
            process.StartInfo.Arguments = "/?";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = false;

            var verbose = false;
            var lockVerify = false;

            // Act
            var result = ProcessExtensions.LockVerify(process, verbose, lockVerify);
            //process.LockVerify(verbose, lockVerify);


            Console.WriteLine($"Output: {result.GetFirstOutput()}");
            // Assert
            Assert.AreEqual(16, result.Code);
            Assert.IsTrue(result.Output.Count > 100);
        }
    }
}