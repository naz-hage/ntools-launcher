using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ntools;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Launcher.Tests
{
    [TestClass()]
    public class SignatureVerifierTests
    {
        private const string MsbuildPath = "MSBuild.exe";


        [TestMethod()]
        public void FileDigitallySignedTest()
        {
            // Arrange
            // This test relies on GitHub Actions to add msbuild.exe it to the path environment variable.
            string msbuildPath = ShellUtility.GetFullPathOfFile(MsbuildPath);
            Console.WriteLine($"MSBuild path: {msbuildPath}");

            // If the MSBuild not added to path, use a default path
            if (!msbuildPath.Contains(MsbuildPath, StringComparison.OrdinalIgnoreCase))
            {
                msbuildPath = @"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64\msbuild.exe";
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