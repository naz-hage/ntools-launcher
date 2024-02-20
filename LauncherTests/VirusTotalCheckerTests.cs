using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ntools.Tests
{
    [TestClass()]
    public class VirusTotalCheckerTests
    {
        private const string ApiKey = "VTAPIKEY";

        [TestMethod]
        public async Task CheckFileAsyncTestAsync()
        {
            // Arrange: Get VT key and download a file to check
            var key = Environment.GetEnvironmentVariable(ApiKey);
            Assert.IsNotNull(key, "key is null");
            
            var file = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe";
            Nfile.SetTrustedHosts(["dist.nuget.org"]);
            Nfile.SetAllowedExtensions([".exe"]);
            var downloadedFile = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "nuget.exe");
            await Nfile.DownloadAsync(file, downloadedFile);
            Assert.IsTrue(File.Exists(downloadedFile), $"{downloadedFile} did not download");

            var checker = new VirusTotalChecker();  

            // Act
            var result = checker.CheckFileAsync(downloadedFile, key).Result;

            // Assert
            Assert.IsNotNull(result, "result is null");

            // Get the result from the VirusTotal API and assert the file is virus free
            Assert.IsTrue(checker.VirusFree, $"{downloadedFile} is not virus free");
        }
    }
}