using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass()]
    public class VirusTotalCheckerTests
    {
        [TestMethod(),Ignore]
        public void CheckFileAsyncTest()
        {
            string key = "fa7d716fa86ad82f76583c3b2b062c60b90b2975ea57559fcc713046f281ce2a"; // Replace with your own key
            // Arrange
            var checker = new VirusTotalChecker();
            var file = @"c:\temp\1.2.57.zip";

            // Act
            var result = checker.CheckFileAsync(file, key).Result;


            // Assert
            Assert.IsNotNull(result);

            // Get the result from the VirusTotal API
            Assert.IsTrue(checker.VirusFree);
        }
    }
}