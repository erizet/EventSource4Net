using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EventSource4Net.Test
{
    [TestClass]
    public class StringSplitterTest
    {
        [TestMethod]
        public void TestDoubleLineFeed()
        {
            string[] lines =
            StringSplitter.SplitIntoLines("test\n\n");

            Assert.AreEqual(lines.Length, 2);
            Assert.AreEqual(lines[0], "test");
            Assert.AreEqual(lines[1], string.Empty);
        }
        [TestMethod]
        public void TestDoubleCarriageReturn()
        {
            string[] lines =
            StringSplitter.SplitIntoLines("test\r\r");

            Assert.AreEqual(lines.Length, 2);
            Assert.AreEqual(lines[0], "test");
            Assert.AreEqual(lines[1], string.Empty);
        }
        [TestMethod]
        public void TestDoubleCarriageReturnLineFeedPair()
        {
            string[] lines =
            StringSplitter.SplitIntoLines("test\r\n\r\n");

            Assert.AreEqual(lines.Length, 2);
            Assert.AreEqual(lines[0], "test");
            Assert.AreEqual(lines[1], string.Empty);
        }
        [TestMethod]
        public void TestTwoLines()
        {
            string[] lines =
            StringSplitter.SplitIntoLines("test\ntest2\n\n");

            Assert.AreEqual(lines.Length, 3);
            Assert.AreEqual(lines[0], "test");
            Assert.AreEqual(lines[1], "test2");
            Assert.AreEqual(lines[2], string.Empty);
        }
        [TestMethod]
        public void TestMixedSeparators()
        {
            string[] lines =
            StringSplitter.SplitIntoLines("test\ntest2\r\ntest3\r\r");

            Assert.AreEqual(lines.Length, 4);
            Assert.AreEqual(lines[0], "test");
            Assert.AreEqual(lines[1], "test2");
            Assert.AreEqual(lines[2], "test3");
            Assert.AreEqual(lines[3], string.Empty);
        }
    }
}
