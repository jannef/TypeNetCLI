using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TypeCLI;

namespace TypeNetCLI.Tests
{
    [TestClass]
    public class NamespaceHelperTests
    {
        [TestMethod]
        public void Split_Happy()
        {
            var split = NamespaceHelper.GetNameSpaces("TypeCLI.Tests");

            Assert.AreEqual("TypeCLI", split[0]);
            Assert.AreEqual("Tests", split[1]);
            Assert.AreEqual(2, split.Length);
        }

        [TestMethod]
        public void Split_ThrowsOnEmptyFragment()
        {
            Assert.ThrowsException<ArgumentException>(() => NamespaceHelper.GetNameSpaces("TypeCLI..Tests"));
            Assert.ThrowsException<ArgumentException>(() => NamespaceHelper.GetNameSpaces("TypeCLI.Tests."));
            Assert.ThrowsException<ArgumentException>(() => NamespaceHelper.GetNameSpaces(".TypeCLI.Tests"));
            Assert.ThrowsException<ArgumentException>(() => NamespaceHelper.GetNameSpaces("TypeCLI. .Tests"));
        }
    }
}