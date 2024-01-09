using Launcher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace launcherTests
{
    [TestClass]
    public class LauncherTests
    {
        private const string ExcecutableToLaunch = "LauncherTest.exe";

        [TestMethod]
        public void ProcessTestRobocopy()
        {
            var result = Launcher.Launcher.Start(new()
                        {
                            WorkingDir = Directory.GetCurrentDirectory(),
                            Arguments = "/?",
                            FileName = "robocopy",
                            RedirectStandardOutput = true
                        }
            );
            Assert.AreEqual(16, result.Code);
            Assert.IsTrue(result.Output.Count > 100);
        }

        [TestMethod]
        public void ProcessStartTestPass()
        {
            
            Parameters launcherParameters = new()
            {
                WorkingDir = Directory.GetCurrentDirectory(),
                Arguments = "pass",
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


            var result = Launcher.Launcher.Start(launcherParameters);
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

            var result = Launcher.Launcher.Start(launcherParameters);


            Assert.AreEqual(-100, result.Code);
            Assert.AreEqual(5, result.Output.Count);
            Assert.IsTrue(result.Output.Contains("fail"));
            Assert.IsTrue(result.Output.Contains("error"));
            Assert.IsTrue(result.Output.Contains("rejected"));
        }

        [TestMethod]
        public void LaunchInThreadTest()
        {
            var result = Launcher.Launcher.LaunchInThread(
                           workingDir: Directory.GetCurrentDirectory(),
                           fileName: ExcecutableToLaunch,
                           arguments: "pass"
                           );
            Assert.AreEqual(0, result.Code);
            Assert.AreEqual("Success", result.Output[0]);

            result = Launcher.Launcher.LaunchInThread(
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
            var result = Launcher.Launcher.Start(launcherParameters);

            Assert.AreEqual(-100, result.Code);
            Assert.AreEqual(5, result.Output.Count);
            Assert.IsTrue(result.Output.Contains("fail"));
            Assert.IsTrue(result.Output.Contains("error"));
            Assert.IsTrue(result.Output.Contains("rejected"));
        }
    }
}