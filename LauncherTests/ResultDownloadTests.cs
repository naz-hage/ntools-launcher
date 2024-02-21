using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ntools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntools.Tests
{
    [TestClass()]
    public class ResultDownloadTests
    {
        [TestMethod()]
        public void ResultDownloadTest()
        {
            // Arrange
            string fileName = "test.txt";
            var uri = new Uri("http://localhost");

            // Act
            var resultDownload = new ResultDownload(uri, fileName);

            // Assert
            Assert.AreEqual(fileName, resultDownload.FileName);
            Assert.AreEqual(uri, resultDownload.Uri);
            Assert.IsFalse(resultDownload.DigitallySigned);
            Assert.AreEqual(0, resultDownload.FileSize);
            Assert.IsNull(resultDownload.X509Certificate2);
            Assert.AreEqual(int.MaxValue, resultDownload.Code);
            Assert.AreEqual(0, resultDownload.Output.Count);
        }
    }
}