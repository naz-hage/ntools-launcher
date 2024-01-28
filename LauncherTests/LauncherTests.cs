using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ntools;
using System;
using System.Diagnostics;
using System.IO;

namespace Launcher.Tests
{
    [TestClass]
    public class LauncherTests
    {
        private const string ExcecutableToLaunch = "LauncherTest.exe";

        [TestMethod]
        public void ProcessTestRobocopy()
        {
            // Arrange
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System),
                    FileName = "robocopy.exe",
                    Arguments = "/?",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = false,
                    UseShellExecute = false
                }
            };

            // Act
            var result = process.LockStart(true);

            // Assert
            Assert.AreEqual(16, result.Code);
            Assert.IsTrue(result.Output.Count > 100);
        }

        [TestMethod]
        public void LaunchInThreadTest()
        {
            var result = Ntools.Launcher.LaunchInThread(
                           workingDir: Directory.GetCurrentDirectory(),
                           fileName: ExcecutableToLaunch,
                           arguments: "pass"
                           );
            Assert.AreEqual(0, result.Code);
            Assert.AreEqual("Success", result.Output[0]);

            result = Ntools.Launcher.LaunchInThread(
               workingDir: Directory.GetCurrentDirectory(),
               fileName: "test1.exe",
               arguments: "fail"
               );
            Assert.AreEqual(int.MinValue, result.Code);
            Assert.IsTrue(result.Output[0].Contains("not found"));
        }

        [TestMethod]
        public void LockStarPassTest()
        {
            // Arrange
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    FileName = ExcecutableToLaunch,
                    Arguments = "pass",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = false
                }
            };

            // Act
            var result = process.LockStart(true);

            // Assert
            Assert.AreEqual(0, result.Code);
            Assert.AreEqual(2, result.Output.Count);
            Assert.IsTrue(result.GetFirstOutput().Contains("pass"));
        }

        [TestMethod]
        public void LockStartFailTest()
        {
            // Arrange
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    FileName = ExcecutableToLaunch,
                    Arguments = "fail",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = false
                }
            };

            // Act
            var result = process.LockStart(true);

            // Assert
            Assert.AreEqual(-100, result.Code);
            Assert.AreEqual(5, result.Output.Count);
            Assert.IsTrue(result.Output.Contains("fail"));
            Assert.IsTrue(result.Output.Contains("error"));
            Assert.IsTrue(result.Output.Contains("rejected"));
        }

        [TestMethod()]
        public void LockVerifyStartTest()
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
   
            // Act
            var result = process.LockVerifyStart(verbose);


            Console.WriteLine($"Output: {result.GetFirstOutput()}");
            // Assert
            Assert.AreEqual(-1, result.Code);
            Assert.IsTrue(result.GetFirstOutput().Contains("is not digitally signed"));
        }

        [TestMethod]
        public void LockStartWithDirectoryNotExist()
        {
            // Arrange
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = @"C:\DoesNotExist",
                    FileName = $"robocopy.exe",
                    Arguments = "/?",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = false
                }
            };
            var verbose = true;

            // Act with LockVerifyStart
            Console.WriteLine("LockVerifyStart");
            var result = process.LockVerifyStart(verbose);

            Console.WriteLine($"Output: {result.GetFirstOutput()}");

            // Assert
            Assert.AreEqual(-1, result.Code);

            // Act with LockVerifyStart
            Console.WriteLine("LockStart");
            result = process.LockVerifyStart(verbose);

            Console.WriteLine($"Output: {result.GetFirstOutput()}");

            // Assert
            Assert.AreEqual(-1, result.Code);
        }

        [TestMethod]
        public void LockStartWithFileNotExist()
        {
            // Arrange
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System),
                    FileName = $"DoesNotExist.exe",
                    Arguments = "/?",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = false
                }
            };
            var verbose = true;

            // Act
            Console.WriteLine("LockVerifyStart");
            var result = process.LockVerifyStart(verbose);

            Console.WriteLine($"Output: {result.GetFirstOutput()}");

            // Assert
            Assert.AreEqual(-1, result.Code);

            // Act with LockVerifyStart
            Console.WriteLine("LockStart");
            result = process.LockVerifyStart(verbose);

            Console.WriteLine($"Output: {result.GetFirstOutput()}");

            // Assert
            Assert.AreEqual(-1, result.Code);
        }
    }
}