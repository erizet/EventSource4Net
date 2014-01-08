using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Collections.Generic;

namespace EventSource4Net.Test
{
    [TestClass]
    public class MessagesTest
    {
        Uri url = new Uri("http://test.com");
        CancellationTokenSource cts;
        List<EventSourceState> states;
        ServiceResponseMock response;
        WebRequesterFactoryMock factory;
        ManualResetEvent stateIsOpen;
        List<ServerSentEvent> receivedMessages;
        ManualResetEvent eventReceived;

        private TestableEventSource SetupAndConnect()
        {
            // setup
            cts = new CancellationTokenSource();
            states = new List<EventSourceState>();
            response = new ServiceResponseMock(url, System.Net.HttpStatusCode.OK);
            factory = new WebRequesterFactoryMock(response);
            stateIsOpen = new ManualResetEvent(false);
            receivedMessages = new List<ServerSentEvent>();
            eventReceived = new ManualResetEvent(false);

            TestableEventSource es = new TestableEventSource(url, factory);
            es.StateChanged += (o, e) =>
            {
                states.Add(e.State);
                if (e.State == EventSourceState.OPEN)
                    stateIsOpen.Set();
            };
            es.EventReceived += (o, e) =>
            {
                receivedMessages.Add(e.Message);
                eventReceived.Set();
            };

            return es;
        }

        [TestMethod]
        public void TestDoubleLineFeed()
        {
            // setup
            TestableEventSource es = SetupAndConnect();

            // act
            es.Start(cts.Token);
            stateIsOpen.WaitOrThrow();
            response.WriteTestTextToStream("event: test\n\n");
            eventReceived.WaitOrThrow();

            // assert
            Assert.AreEqual(receivedMessages.Count, 1);
            Assert.AreEqual(receivedMessages[0].EventType, "test");
        }
        [TestMethod]
        public void TestDoubleCarriageReturn()
        {
            // setup
            TestableEventSource es = SetupAndConnect();

            // act
            es.Start(cts.Token);
            stateIsOpen.WaitOrThrow();
            response.WriteTestTextToStream("event: test\r\r");
            eventReceived.WaitOrThrow();

            // assert
            Assert.AreEqual(receivedMessages.Count, 1);
            Assert.AreEqual(receivedMessages[0].EventType, "test");
        }
        [TestMethod]
        public void TestDoubleCarriageReturnLineFeedPair()
        {
            // setup
            TestableEventSource es = SetupAndConnect();

            // act
            es.Start(cts.Token);
            stateIsOpen.WaitOrThrow();
            response.WriteTestTextToStream("event: test\r\n\r\n");
            eventReceived.WaitOrThrow();

            // assert
            Assert.AreEqual(receivedMessages.Count, 1);
            Assert.AreEqual(receivedMessages[0].EventType, "test");
        }
        [TestMethod]
        public void TestTwoLines()
        {
            // setup
            TestableEventSource es = SetupAndConnect();

            // act
            es.Start(cts.Token);
            stateIsOpen.WaitOrThrow();
            response.WriteTestTextToStream("event: test\ndata: simple\n\n");
            eventReceived.WaitOrThrow();

            // assert
            Assert.AreEqual(receivedMessages.Count, 1);
            Assert.AreEqual(receivedMessages[0].EventType, "test");
            Assert.AreEqual(receivedMessages[0].Data, "simple\n");
        }
        [TestMethod]
        public void TestMixedSeparators()
        {
            // setup
            TestableEventSource es = SetupAndConnect();

            // act
            es.Start(cts.Token);
            stateIsOpen.WaitOrThrow();
            response.WriteTestTextToStream("event: test\rdata: simple\n\n");
            eventReceived.WaitOrThrow();

            // assert
            Assert.AreEqual(receivedMessages.Count, 1);
            Assert.AreEqual(receivedMessages[0].EventType, "test");
            Assert.AreEqual(receivedMessages[0].Data, "simple\n");
        }
        [TestMethod]
        public void TestDataSentInParts()
        {
            // setup
            TestableEventSource es = SetupAndConnect();

            // act
            es.Start(cts.Token);
            stateIsOpen.WaitOrThrow();
            response.WriteTestTextToStream("event: tes");
            Thread.Sleep(10);
            response.WriteTestTextToStream("t\ndata: simple\n\n");
            eventReceived.WaitOrThrow();

            // assert
            Assert.AreEqual(receivedMessages.Count, 1);
            Assert.AreEqual(receivedMessages[0].EventType, "test");
            Assert.AreEqual(receivedMessages[0].Data, "simple\n");
        }
        [TestMethod]
        public void TestMultipleEvents()
        {
            // setup
            TestableEventSource es = SetupAndConnect();

            // act
            es.Start(cts.Token);
            stateIsOpen.WaitOrThrow();

            //entire event in one line
            response.WriteTestTextToStream("id: 1" + "\n" + "event: newevent" + "\n" + "data: Hello" + "\n\n");
            eventReceived.WaitOrThrow();
            eventReceived.Reset();

            //This event is split up over multiple res.write lines 
            response.WriteTestTextToStream("id:2 " + "\n");
            response.WriteTestTextToStream("event: event 3" + "\n");
            response.WriteTestTextToStream("data: Hello again");
            response.WriteTestTextToStream("\n\n");
            eventReceived.WaitOrThrow();
            eventReceived.Reset();

            //Again an event that is split up over multiple res.write statements
            response.WriteTestTextToStream("id: 3" + "\n");
            response.WriteTestTextToStream("event: event3" + "\n" + "data: Goodbye" + "\n\n");
            eventReceived.WaitOrThrow();
            eventReceived.Reset();

            // assert
            Assert.AreEqual(receivedMessages.Count, 3);
        }
    }
}
