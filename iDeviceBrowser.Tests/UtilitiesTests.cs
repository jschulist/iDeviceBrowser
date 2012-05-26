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

        // TODO: THIS SHOULD LIKELY RETURN EMPTY.STRING TO MATCH PATH.COMBINE OF SYSTEM.IO
        [TestMethod]
        public void PathCombine_Empty_Empty_ReturnsSlash()
        {
            Assert.AreEqual("/", Utilities.PathCombine(string.Empty, string.Empty));
        }

        // TODO: THIS SHOULD LIKELY RETURN bin TO MATCH PATH.COMBINE OF SYSTEM.IO
        [TestMethod]
        public void PathCombine_Empty_bin_ReturnsSlash()
        {
            Assert.AreEqual("/bin", Utilities.PathCombine(string.Empty, "bin"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PathCombine_Empty_Slashbin_ReturnsSlash()
        {
            Assert.AreEqual("/bin", Utilities.PathCombine(string.Empty, "/bin"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PathCombine_Empty_SlashbinSlash_ReturnsSlash()
        {
            Assert.AreEqual("/bin/", Utilities.PathCombine(string.Empty, "/bin/"));
        }

        [TestMethod]
        public void PathCombine_Slash_Empty_ReturnsSlash()
        {
            Assert.AreEqual("/", Utilities.PathCombine("/", string.Empty));
        }

        [TestMethod]
        public void PathCombine_Slash_bin_ReturnsSlash()
        {
            Assert.AreEqual("/bin", Utilities.PathCombine("/", "bin"));
        }

        [TestMethod]
        public void PathCombine_Slashbin_apt_ReturnsSlash()
        {
            Assert.AreEqual("/bin/apt", Utilities.PathCombine("/bin", "apt"));
        }

        [TestMethod]
        public void PathCombine_SlashbinSlash_apt_ReturnsSlash()
        {
            Assert.AreEqual("/bin/apt", Utilities.PathCombine("/bin/", "apt"));
        }

        [TestMethod]
        public void PathCombine_bin_apt_ReturnsSlash()
        {
            Assert.AreEqual("bin/apt", Utilities.PathCombine("bin", "apt"));
        }
    }
}
