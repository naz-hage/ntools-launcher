using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography.X509Certificates;
using System;
using System.IO;

namespace Launcher.Tests
{
    [TestClass()]
    public class DigitalSignatureHelperTests
    {
        private const string ExcecutableToLaunch = @"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64\msbuild.exe";
        [TestMethod()]
        public void FileDigitallySignedTest()
        {
            // Arrange
            var fileStream = new System.IO.FileStream(ExcecutableToLaunch, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);


            // Act
            var result = SignatureVerifier.VerifyDigitalSignature(fileStream.Name);


            // Assert
            Assert.IsTrue(result);

            DisplayCertificate(fileStream.Name);
        }

        public static void DisplayCertificate(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));
            }

            if (!File.Exists(fileName))
            {
                throw new ArgumentException("File does not exist.", nameof(fileName));
            }

            // Create an X509Certificate2 object to represent the certificate of the signed file.
            var cert = new X509Certificate2(fileName);

            // Display the properties of the certificate.
            Console.WriteLine($"SignatureAlgorithm: {cert.SignatureAlgorithm.FriendlyName.ToString()}");
            Console.WriteLine($"Subject: {cert.Subject}");
            Console.WriteLine($"Issuer: {cert.Issuer}");
            Console.WriteLine($"Version: {cert.Version}");
            Console.WriteLine($"Valid From: {cert.NotBefore}");
            Console.WriteLine($"Valid To: {cert.NotAfter}");
            Console.WriteLine($"Serial Number: {cert.SerialNumber}");
            Console.WriteLine($"Thumbprint: {cert.Thumbprint}");
        }
    }
}