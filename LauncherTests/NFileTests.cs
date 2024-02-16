using Ntools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO.Pipes;

namespace Ntools.Tests
{
    [TestClass()]
    public class NFileTests
    {
        [TestMethod()]
        public async Task DownloadAsyncTest()
        {
            // Arrange
            var httpClient = new HttpClient();
            Uri webDownloadFile = new("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe");
            string downloadedFile = "nuget.exe";

            // setup file name to download to temp folder because devtools is protected
            downloadedFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(downloadedFile));
            if (File.Exists(downloadedFile))
            {
                File.Delete(downloadedFile);
            }

            // Act
            var result = await httpClient.DownloadAsync(webDownloadFile, downloadedFile);

            // Assert
            Assert.IsTrue(File.Exists(downloadedFile));

            Console.WriteLine($"File size: {result.FileSize}");
            Console.WriteLine($"File is signed: {result.DigitallySigned}");
            result.DisplayCertificate();

            Assert.IsTrue(result.IsSuccess());
            Assert.IsTrue(result.GetFirstOutput().Contains("Success"));
        }

        [TestMethod()]
        public async Task DownloadFileTaskAsyncValidateParametersTestAsync()
        {
            // Arrange
            var expectedFail = new Dictionary<Uri, string>
            {
                {   new("https://desktop.docker.com/win/main/am@d64/Docker%20Desktop%20Installer.exe"),"Docker.Desktop.Installer.exe" },
                {   new("http://desktop.docker.com/win/main/amd64/Docker%20Desktop%20Installer.exe"), "Docker.Desktop.Installer.exe" },
                {   new("https://desktop.docker.com/win/main/amd64/Docker%20Desktop%20Installer.exe"), "Docker.Desk>top.Installer.exe" },
            };

            var httpClient = new HttpClient();

            foreach (var item in expectedFail)
            {
                // setup file name to download to temp folder because devtools is protected
                var fileName = Path.Combine(Path.GetTempPath(), Path.GetFileName(item.Value));
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Act
                var result = await httpClient.DownloadAsync(item.Key, fileName);


                Console.WriteLine($"output: {result.GetFirstOutput()}");

                // Assert
                Assert.IsFalse(result.IsSuccess());
            }
        }

        [TestMethod()]
        public async Task GetFileSizeAsyncTest()
        {
            // Arrange
            var httpClient = new HttpClient();
            Uri webDownloadFile = new("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe");
            string downloadedFile = "nuget.exe";

            // setup file name to download to temp folder because devtools is protected
            downloadedFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(downloadedFile));
            if (File.Exists(downloadedFile))
            {
                File.Delete(downloadedFile);
            }

            // Act
            var result = await httpClient.GetFileSizeAsync(webDownloadFile.ToString());
            Console.WriteLine($"File size: {result}");

            // Assert
            Assert.IsTrue(result > 0);
        }

        [TestMethod()]
        public async Task UriExistsAsyncTest()
        {
            // Arrange
            var httpClient = new HttpClient();
            Uri webDownloadFile = new("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe");
            string downloadedFile = "nuget.exe";

            // setup file name to download to temp folder because devtools is protected
            downloadedFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(downloadedFile));
            if (File.Exists(downloadedFile))
            {
                File.Delete(downloadedFile);
            }

            // Act
            var result = await httpClient.UriExistsAsync(webDownloadFile.ToString());
            Console.WriteLine($"Uri Exist: {result}");

            // Assert
            Assert.IsTrue(result);
        }
    }
}
