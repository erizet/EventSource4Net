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
            string remainingText = string.Empty;
            string[] lines =
            StringSplitter.SplitIntoLines("test\n\n", out remainingText);

            Assert.AreEqual(lines.Length, 2);
            Assert.AreEqual(lines[0], "test");
            Assert.AreEqual(lines[1], string.Empty);
        }
        [TestMethod]
        public void TestDoubleCarriageReturn()
        {
            string remainingText = string.Empty;
            string[] lines =
            StringSplitter.SplitIntoLines("test\r\r", out remainingText);

            Assert.AreEqual(lines.Length, 2);
            Assert.AreEqual(lines[0], "test");
            Assert.AreEqual(lines[1], string.Empty);
        }
        [TestMethod]
        public void TestDoubleCarriageReturnLineFeedPair()
        {
            string remainingText = string.Empty; 
            string[] lines =
            StringSplitter.SplitIntoLines("test\r\n\r\n", out remainingText);

            Assert.AreEqual(lines.Length, 2);
            Assert.AreEqual(lines[0], "test");
            Assert.AreEqual(lines[1], string.Empty);
        }
        [TestMethod]
        public void TestTwoLines()
        {
            string remainingText = string.Empty;
            string[] lines =
            StringSplitter.SplitIntoLines("test\ntest2\n\n", out remainingText);

            Assert.AreEqual(lines.Length, 3);
            Assert.AreEqual(lines[0], "test");
            Assert.AreEqual(lines[1], "test2");
            Assert.AreEqual(lines[2], string.Empty);
        }
        [TestMethod]
        public void TestMixedSeparators()
        {
            string remainingText = string.Empty;
            string[] lines =
            StringSplitter.SplitIntoLines("test\ntest2\r\ntest3\r\r", out remainingText);

            Assert.AreEqual(lines.Length, 4);
            Assert.AreEqual(lines[0], "test");
            Assert.AreEqual(lines[1], "test2");
            Assert.AreEqual(lines[2], "test3");
            Assert.AreEqual(lines[3], string.Empty);
            Assert.AreEqual(remainingText, string.Empty);
        }
        [TestMethod]
        public void TestRemainingTextNoCompleteRow()
        {
            string remainingText = string.Empty;
            string[] lines =
            StringSplitter.SplitIntoLines("test", out remainingText);

            Assert.AreEqual(lines.Length, 0);
            Assert.AreEqual(remainingText, "test");
        }
        [TestMethod]
        public void TestRemainingTextAfterOneRow()
        {
            string remainingText = string.Empty;
            string[] lines =
            StringSplitter.SplitIntoLines("test\ntest2", out remainingText);

            Assert.AreEqual(lines.Length, 1);
            Assert.AreEqual(remainingText, "test2");
        }
    }
}
