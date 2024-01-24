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
            var result = Ntools.Launcher.Start(new()
                        {
                            WorkingDir = Environment.GetFolderPath(Environment.SpecialFolder.System),
                            Arguments = "/?",
                            FileName = "robocopy.exe",
                            RedirectStandardOutput = true
                        }
            );
            Assert.AreEqual(16, result.Code);
            Assert.IsTrue(result.Output.Count > 100);
        }

        [TestMethod]
        public void ProcessStartTestPass()
        {

            Parameters Parameters = new()
            {
                WorkingDir = Directory.GetCurrentDirectory(),
                Arguments = "pass",
                FileName = ExcecutableToLaunch,
                RedirectStandardOutput = true
            };

            Console.WriteLine($"WorkingDir: {Parameters.WorkingDir}");
            Console.WriteLine($"FileName: {Parameters.FileName}");
            Console.WriteLine($"Arguments: {Parameters.Arguments}");
            Console.WriteLine($"RedirectStandardOutput: {Parameters.RedirectStandardOutput}");
            var expectedExcecutablePath = Path.Combine(Path.GetFullPath(Parameters.WorkingDir), Parameters.FileName);
            Console.WriteLine($"expectedExcecutablePath: {expectedExcecutablePath}");
            Assert.IsTrue(File.Exists(expectedExcecutablePath));


            var result = Ntools.Launcher.Start(Parameters);
            Assert.AreEqual(0, result.Code);
            Assert.AreEqual(2, result.Output.Count);
        }

        [TestMethod]
        public void ProcessStartTestFail()
        {
            Parameters launcherParameters = new()
            {
                WorkingDir = Directory.GetCurrentDirectory(),
                Arguments = "fail",
                FileName = ExcecutableToLaunch,
                RedirectStandardOutput = true
            };

            Console.WriteLine($"WorkingDir: {launcherParameters.WorkingDir}");
            Console.WriteLine($"FileName: {launcherParameters.FileName}");
            Console.WriteLine($"Arguments: {launcherParameters.Arguments}");
            Console.WriteLine($"RedirectStandardOutput: {launcherParameters.RedirectStandardOutput}");
            var expectedExcecutablePath = Path.Combine(Path.GetFullPath(launcherParameters.WorkingDir), launcherParameters.FileName);
            Console.WriteLine($"expectedExcecutablePath: {expectedExcecutablePath}");
            Assert.IsTrue(File.Exists(expectedExcecutablePath));

            var result = Ntools.Launcher.Start(launcherParameters);


            Assert.AreEqual(-100, result.Code);
            foreach (var line in result.Output)
            {
                Console.WriteLine(line);
            }

            Assert.AreEqual(5, result.Output.Count);
            Assert.IsTrue(result.Output.Contains("fail"));
            Assert.IsTrue(result.Output.Contains("error"));
            Assert.IsTrue(result.Output.Contains("rejected"));
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
        public void ProcessStartTestWithLauncherParameters()
        {
            Parameters launcherParameters = new()
            {
                WorkingDir = Directory.GetCurrentDirectory(),
                FileName = ExcecutableToLaunch,
                Arguments = "fail",
                RedirectStandardOutput = true
            };
            Console.WriteLine($"WorkingDir: {launcherParameters.WorkingDir}");
            Console.WriteLine($"FileName: {launcherParameters.FileName}");
            Console.WriteLine($"Arguments: {launcherParameters.Arguments}");
            Console.WriteLine($"RedirectStandardOutput: {launcherParameters.RedirectStandardOutput}");
            var expectedExcecutablePath = Path.Combine(Path.GetFullPath(launcherParameters.WorkingDir), launcherParameters.FileName);
            Console.WriteLine($"expectedExcecutablePath: {expectedExcecutablePath}");
            Assert.IsTrue(File.Exists(expectedExcecutablePath));
            var result = Ntools.Launcher.Start(launcherParameters);

            Assert.AreEqual(-100, result.Code);
            Assert.AreEqual(5, result.Output.Count);
            Assert.IsTrue(result.Output.Contains("fail"));
            Assert.IsTrue(result.Output.Contains("error"));
            Assert.IsTrue(result.Output.Contains("rejected"));
        }

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
            var result = process.LockVerifyStart(verbose, lockVerify);


            Console.WriteLine($"Output: {result.GetFirstOutput()}");
            // Assert
            Assert.AreEqual(16, result.Code);
            Assert.IsTrue(result.Output.Count > 100);
        }
    }
}