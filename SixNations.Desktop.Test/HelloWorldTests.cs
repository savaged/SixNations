using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SixNations.Desktop.Test
{
    [TestClass]
    public class HelloWorldTests
    {
        [TestMethod]
        public void HelloWorldTest()
        {
            Assert.IsTrue(true, "Hello World!");
        }
    }
}
