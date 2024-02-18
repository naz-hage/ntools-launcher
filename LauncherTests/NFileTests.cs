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

            Nfile.SetTrustedHosts(new List<string> { "dist.nuget.org" });
            Nfile.SetAllowedExtensions(new List<string> { ".exe" });

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
        public async Task DownloadFileUriNotFoundTestAsync()
        {
            // Arrange
            var expectedFail = new Dictionary<Uri, string>
            {
                {   new("https://desktop.docker.com/win/main/am@d64/Docker%20Desktop%20Installer.exe"),"Docker.Desktop.Installer.exe" }, // Uri not found Result no success
            };
            Nfile.SetTrustedHosts(new List<string> { "desktop.docker.com" });

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
        public async Task DownloadFileInvalidUriExceptionTestAsync()
        {
            // Arrange
            var expectedFail = new Dictionary<Uri, string>
            {
                {   new("http://desktop.docker.com/win/main/amd64/Docker%20Desktop%20Installer.exe"), "Docker.Desktop.Installer.exe" },  // Invalid Uri: exception 
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

                // Act and Assert
                await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await httpClient.DownloadAsync(item.Key, fileName));
            }
        }

        [TestMethod()]
        public async Task DownloadFileInvalidDownloadedFienameExceptionTestAsync()
        {
            // Arrange
            var expectedFail = new Dictionary<Uri, string>
            {
                { new("https://desktop.docker.com/win/main/amd64/Docker%20Desktop%20Installer.exe"), "Docker.Desk>top.Installer.exe" },  //Invalid download filename: exception
                { new("https://desktop.docker.com/win/main/amd64/Docker%20Desktop%20Installer2.exe"), "c:\\temp\\Docker.Desk>top.Installer.exe" },  //Invalid download filename: exception
                { new("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"), null },  //Invalid download filename: exception
            };

            var httpClient = new HttpClient();

            foreach (var item in expectedFail)
            {
                // Act and Assert
                await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await httpClient.DownloadAsync(item.Key, item.Value));
            }
        }

        [TestMethod]
        public void ValidUriTest()
        {
            // Arrange
            string fileName = "c:\\temp\\test.txt";
            Uri uri = new Uri("https://localhost");

            // Act
            ResultDownload resultDownload = new ResultDownload(fileName, uri);

            // Assert
            Assert.AreEqual(fileName, resultDownload.FileName);
            Assert.AreEqual(uri, resultDownload.Uri);
        }
       
        [TestMethod]
        public async Task SetAllowedExtensionsTestAsync()
        {
            // Arrange
            var allowedExtensions = new List<string> { ".exe", ".msi" };

            // Act
            Nfile.SetAllowedExtensions(allowedExtensions);

            // Assert
            Assert.AreEqual(allowedExtensions, Nfile.GetAllowedExtensions());

            // download file with allowed extension
            var httpClient = new HttpClient();
            Uri webDownloadFile = new("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe");
            string downloadedFile = "nuget.exe";
            
            Nfile.SetTrustedHosts(new List<string> { "dist.nuget.org" });
            // setup file name to download to temp folder because devtools is protected
            downloadedFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(downloadedFile));
            if (File.Exists(downloadedFile))
            {
                File.Delete(downloadedFile);
            }

            // Act
            var result = httpClient.DownloadAsync(webDownloadFile, downloadedFile).Result;

            // Assert
            Assert.IsTrue(File.Exists(downloadedFile));

            Console.WriteLine($"File size: {result.FileSize}");

            Assert.IsTrue(result.IsSuccess());
            Assert.IsTrue(result.GetFirstOutput().Contains("Success"));

            // download file with not allowed extension

            webDownloadFile = new("https://dist.nuget.org/win-x86-commandline/latest/nuget.zip");
            downloadedFile = "nuget.zip";

            // setup file name to download to temp folder because devtools is protected
            downloadedFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(downloadedFile));
            if (File.Exists(downloadedFile))
            {
                File.Delete(downloadedFile);
            }

            // Act
            try
            {
                result = await httpClient.DownloadAsync(webDownloadFile, downloadedFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");

                // Assert
                Assert.IsTrue(ex.Message.Contains("Invalid uri extension"));
            }
            
            // Act and Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await httpClient.DownloadAsync(webDownloadFile, downloadedFile));
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
            Assert.IsTrue(result.IsSuccessStatusCode);
        }

        [TestMethod()]
        public async Task DownloadAsyncUriNotFoundTest()
        {
            // Arrange
            var httpClient = new HttpClient();
            Uri webDownloadFile = new("https://dist.nuget.org/win-x86-commandline-notFound/latest/nuget.exe");
            string downloadedFile = "nuget.exe";

            // setup file name to download to temp folder because devtools is protected
            downloadedFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(downloadedFile));
            if (File.Exists(downloadedFile))
            {
                File.Delete(downloadedFile);
            }

            // Act
            var result = await httpClient.DownloadAsync(webDownloadFile, downloadedFile);
            Console.WriteLine($"http Response:\n {result.GetFirstOutput()}");

            // Assert
            Assert.IsFalse(result.IsSuccess());
        }
    }
}
