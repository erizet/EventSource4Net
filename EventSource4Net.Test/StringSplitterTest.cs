using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Collections.Generic;

namespace EventSource4Net.Test
{
    [TestClass]
    public class StringSplitterTest
    {
        Uri url = new Uri("http://test.com");
        CancellationTokenSource cts;
        List<EventSourceState> states;
        ServiceResponseMock response;
        WebRequesterFactoryMock factory;
        ManualResetEvent stateIsOpen;

        
        private TestableEventSource SetupAndConnect()
        {
            // setup
            cts = new CancellationTokenSource();
            states = new List<EventSourceState>();
            response = new ServiceResponseMock(url, System.Net.HttpStatusCode.OK);
            factory = new WebRequesterFactoryMock(response);
            stateIsOpen = new ManualResetEvent(false);

            TestableEventSource es = new TestableEventSource(url, factory);
            es.StateChanged += (o, e) =>
            {
                states.Add(e.State);
                if (e.State == EventSourceState.OPEN)
                    stateIsOpen.Set();
            };

            return es;
        }

        [TestMethod]
        public void TestDoubleLineFeed()
        {
            // setup
            TestableEventSource es = SetupAndConnect();

            List<string> receivedMessages = new List<string>();
            ManualResetEvent eventReceived = new ManualResetEvent(false);
            es.EventReceived += (o, e) => 
            {
                receivedMessages.Add(e.Message.Data);
                eventReceived.Set();
            };
            
            // act
            es.Start(cts.Token);
            stateIsOpen.WaitOne();
            response.WriteTestTextToStream("test\n\n");
            eventReceived.WaitOne();

            // assert
            Assert.AreEqual(receivedMessages.Count, 1);
            Assert.AreEqual(receivedMessages[0], "test");
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
