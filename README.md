[nuget]: https://nuget.org/packages/EventSource4Net
[slf4net]: https://github.com/englishtown/slf4net
EventSource4Net
===============

EventSource4Net is a eventsource implementation for .Net. By using EventSource4Net can you receive Server-Sent Event(SSE) in your native .Net program.

##Install##
EventSource4Net is available as a [Nuget-package][nuget]. From the Package Manager Console enter:
            
            Install-package EventSource4Net

##How to use?##

It's dead-simple to use.

            EventSource es = new EventSource(new Uri(<Your url>));
            es.StateChanged += new EventHandler<StateChangedEventArgs>((o, e) => { Console.WriteLine("New state: " + e.State.ToString()); });
            es.EventReceived += new EventHandler<ServerSentEventReceivedEventArgs>((o, e) => { Console.WriteLine("--------- Msg received -----------\n" + e.Message.ToString()); });
            es.Start();

See the sample-project!

##Logging##
EventSource4Net uses [slf4net] as a logging facade.

##ToDo##
- Implement functionallity to cancel the eventsource.

##Contributions##
I'll be more than happy to get contributions!!!
