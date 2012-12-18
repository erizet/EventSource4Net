EventSource4Net
===============

EventSource4Net is a eventsource implementation for .Net. By using EventSource4Net can you receive Server-Sent Event(SSE) in your native .Net program.

##How to use?##

It's dead-simple to use.

            EventSource es = new EventSource(new Uri(<Your url>);
            es.StateChanged += new EventHandler<StateChangedEventArgs>((o, e) => { Console.WriteLine("New state: " + e.State.ToString()); });
            es.EventReceived += new EventHandler<ServerSentEventReceivedEventArgs>((o, e) => { Console.WriteLine("--------- Msg received -----------\n" + e.Message.ToString()); });
            es.Start();

See the sample-project!

##ToDo##
- Implement functionallity to cancel the eventsource.
- Add a logging facade

##Contributions##
I'll be more than happy to get contributions!!!
