using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Ntools.Tests
{
    [TestClass()]
    [DoNotParallelize]
    public class SignatureVerifierTests
    {
        private const string MsbuildPath = "MSBuild.exe";


        [TestMethod()]
        public void FileDigitallySignedTest()
        {
            // Arrange
            // This test relies on GitHub Actions to add msbuild.exe it to the path environment variable.
            string msbuildPath = ShellUtility.GetFullPathOfFile(MsbuildPath);
            Console.WriteLine($"MSBuild path from PATH: {msbuildPath}");

            // If the MSBuild not added to path, try hardcoded paths for VS 2022 and 2026
            if (!msbuildPath.Contains(MsbuildPath, StringComparison.OrdinalIgnoreCase))
            {
                string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                
                // Try VS 2022 first
                msbuildPath = System.IO.Path.Combine(programFiles, @"Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64\msbuild.exe");
                if (!System.IO.File.Exists(msbuildPath))
                {
                    // Try VS 2026
                    msbuildPath = System.IO.Path.Combine(programFiles, @"Microsoft Visual Studio\2026\Community\MSBuild\Current\Bin\amd64\msbuild.exe");
                }
            }

            // Check if file exists before proceeding
            if (!System.IO.File.Exists(msbuildPath))
            {
                Assert.Inconclusive($"MSBuild.exe not found at expected locations. Searched: VS 2022 and VS 2026 Community editions.");
                return;
            }

            var fileStream = new System.IO.FileStream(msbuildPath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);

            // Act
            var result = SignatureVerifier.VerifyDigitalSignature(fileStream.Name);

            // Assert
            Assert.IsTrue(result);

            SignatureVerifier.DisplayCertificate(fileStream.Name);
        }
    }
}