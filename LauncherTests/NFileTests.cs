using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ntools.Tests
{
    [TestClass()]
    public class NfileTests
    {
        [TestMethod()]
        public async Task DownloadAsyncTest()
        {
            // Arrange
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
            var result = await Nfile.DownloadAsync(webDownloadFile.ToString(), downloadedFile);

            // Assert
            Assert.IsTrue(File.Exists(downloadedFile));

            Console.WriteLine($"File size: {result.FileSize}");
            Console.WriteLine($"File is signed: {result.DigitallySigned}");
            Assert.IsTrue(result.DigitallySigned);
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
            Nfile.SetAllowedExtensions(new List<string> { ".exe" });

            

            foreach (var item in expectedFail)
            {
                // setup file name to download to temp folder because devtools is protected
                var fileName = Path.Combine(Path.GetTempPath(), Path.GetFileName(item.Value));
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                try
                {
                    // Act
                    var result = await Nfile.DownloadAsync(item.Key.ToString(), fileName);
                    Console.WriteLine($"output: {result.GetFirstOutput()}");
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex.Message.Contains("The remote server returned an error: (403) Forbidden"));
                }
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

            

            foreach (var item in expectedFail)
            {
                // setup file name to download to temp folder because devtools is protected
                var fileName = Path.Combine(Path.GetTempPath(), Path.GetFileName(item.Value));
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Act and Assert
                await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await Nfile.DownloadAsync(item.Key.ToString(), fileName));
            }
        }

        [TestMethod()]
        public async Task DownloadFileInvalidDownloadedFilenameExceptionTestAsync()
        {
            // Arrange
            var expectedFail = new Dictionary<Uri, string>
            {
                { new("https://desktop.docker.com/win/main/amd64/Docker%20Desktop%20Installer.exe"), "Docker.Desk>top.Installer.exe" },  //Invalid download filename: exception
                { new("https://github.com/naz-hage/ntools/releases/download/1.3.0/1.3.0.zip"), "c:\\temp\\Docker.Desk>top.Installer.exe" },  //Invalid download filename: exception
                { new("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"), null },  //Invalid download filename: exception
            };

            

            Nfile.SetTrustedHosts(new List<string> { "desktop.docker.com", "github.com", "dist.nuget.org" });

            foreach (var item in expectedFail)
            {
                Console.WriteLine($"Uri: {item.Key}");
                Console.WriteLine($"Downloaded file: {item.Value}");
                // Act and Assert
                await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await Nfile.DownloadAsync(item.Key.ToString(), item.Value));
            }
        }

        [TestMethod]
        public void ValidUriTest()
        {
            // Arrange
            string fileName = "c:\\temp\\test.txt";
            Uri uri = new Uri("https://localhost");

            // Act
            ResultDownload resultDownload = new ResultDownload(uri, fileName);

            // Assert
            Assert.AreEqual(fileName, resultDownload.FileName);
            Assert.AreEqual(uri, resultDownload.Uri);
        }
       
        [TestMethod]
        public void SetAllowedExtensionsTestAsync()
        {
            // Arrange
            var allowedExtensions = new List<string> { ".exe", ".msi" };

            // Act
            Nfile.SetAllowedExtensions(allowedExtensions);

            // Assert
            Assert.AreEqual(allowedExtensions, Nfile.GetAllowedExtensions());

        }

       
        [TestMethod]
        public async Task SetAllowedExtensionsNullTestAsync()
        {
            // Arrange download file with not allowed extension
            
            Uri webDownloadFile = new("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe");
            string downloadedFile = "nuget.exe";

            Nfile.SetTrustedHosts(new List<string> { "dist.nuget.org" });
            // setup file name to download to temp folder because devtools is protected
            downloadedFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(downloadedFile));
            if (File.Exists(downloadedFile))
            {
                File.Delete(downloadedFile);
            }
            Nfile.SetAllowedExtensions(new List<string>());

            // Act
            try
            {
                var result = await Nfile.DownloadAsync(webDownloadFile.ToString(), downloadedFile);
                Console.WriteLine($"Download Response:\n {result.GetFirstOutput()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");

                // Assert
                Assert.IsTrue(ex.Message.Contains("Invalid uri extension"));
            }

            // Act and Assert
            //await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await Nfile.DownloadAsync(webDownloadFile, downloadedFile));
        }

        [TestMethod()]
        public async Task GetFileSizeAsyncTest()
        {
            // Arrange
            
            Uri webDownloadFile = new("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe");
            string downloadedFile = "nuget.exe";

            // setup file name to download to temp folder because devtools is protected
            downloadedFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(downloadedFile));
            if (File.Exists(downloadedFile))
            {
                File.Delete(downloadedFile);
            }

            // Act
            var result = await Nfile.GetFileSizeAsync(webDownloadFile.ToString());
            Console.WriteLine($"File size: {result}");

            // Assert
            Assert.IsTrue(result > 0);
        }

        [TestMethod()]
        public async Task UriExistsAsyncTest()
        {
            // Arrange
            
            Uri webDownloadFile = new("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe");
            string downloadedFile = "nuget.exe";

            // setup file name to download to temp folder because devtools is protected
            downloadedFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(downloadedFile));
            if (File.Exists(downloadedFile))
            {
                File.Delete(downloadedFile);
            }

            // Act
            var result = await Nfile.UriExistsAsync(webDownloadFile.ToString());
            Console.WriteLine($"Uri Exist: {result}");

            // Assert
            Assert.IsTrue(result.IsSuccessStatusCode);
        }

        [TestMethod()]
        public async Task DownloadAsyncUriNotFoundTest()
        {
            // Arrange
            
            Uri webDownloadFile = new("https://dist.nuget.org/win-x86-commandline-notFound/latest/nuget.exe");
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
            try
            {
                var result = await Nfile.DownloadAsync(webDownloadFile.ToString(), downloadedFile);
                Console.WriteLine($"http Response:\n {result.GetFirstOutput()}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Assert.IsTrue(ex.Message.Contains("The remote server returned an error: (404) Not Found"));

                // Assert
                //Assert.IsFalse(result.IsSuccess());
            }
        }
    }
}
