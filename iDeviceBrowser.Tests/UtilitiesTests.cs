using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iDeviceBrowser.Tests
{
    [TestClass]
    public class UtilitiesTests
    {
        [TestMethod]
        public void GetFileSize_0_Returns0KB()
        {
            Assert.AreEqual("0 KB", Utilities.GetFileSize(0));
        }

        [TestMethod]
        public void GetFileSize_1_Returns1KB()
        {
            Assert.AreEqual("1 KB", Utilities.GetFileSize(1));
        }

        [TestMethod]
        public void GetFileSize_1024_Returns1KB()
        {
            Assert.AreEqual("1 KB", Utilities.GetFileSize(1024));
        }

        [TestMethod]
        public void GetFileSize_10240_Returns10KB()
        {
            Assert.AreEqual("10 KB", Utilities.GetFileSize(10240));
        }

        [TestMethod]
        public void GetFileSize_1024000_Returns1000KB()
        {
            Assert.AreEqual("1,000 KB", Utilities.GetFileSize(1024000));
        }
    }
}
