using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSource4Net;
using slf4net;
using slf4net.Resolvers;
using System.Threading;


namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            InitLogging();

            EventSource es = new EventSource(new Uri(@"http://ssetest.apphb.com/api/sse"), 50000);
            es.StateChanged += new EventHandler<StateChangedEventArgs>((o, e) => { Console.WriteLine("New state: " + e.State.ToString()); });
            es.EventReceived += new EventHandler<ServerSentEventReceivedEventArgs>((o, e) => { Console.WriteLine("--------- Msg received -----------\n" + e.Message.ToString()); });
            es.Start(cts.Token);
            Console.WriteLine("EventSource started");

            ConsoleKey key;
            while ((key = Console.ReadKey().Key) != ConsoleKey.X)
            {
                if (key == ConsoleKey.C)
                {
                    cts.Cancel();
                    Console.WriteLine("Eventsource is cancelled.");
                }
                else if (key==ConsoleKey.S)
                {
                    cts = new CancellationTokenSource();
                    es.Start(cts.Token);
                }
            }
        }

        private static void InitLogging()
        {
            // Create log4net ILoggerFactory and set the resolver
            var factory = new slf4net.Factories.SimpleLoggerFactory(new TraceLogger("Test"));
            var resolver = new SimpleFactoryResolver(factory);
            LoggerFactory.SetFactoryResolver(resolver);
        }
    }
}
