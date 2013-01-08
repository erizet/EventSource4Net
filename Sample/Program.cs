using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSource4Net;
using slf4net;
using slf4net.Resolvers;


namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            InitLogging();

            EventSource es = new EventSource(new Uri(@"http://ssetest.apphb.com/api/sse"));
            es.StateChanged += new EventHandler<StateChangedEventArgs>((o, e) => { Console.WriteLine("New state: " + e.State.ToString()); });
            es.EventReceived += new EventHandler<ServerSentEventReceivedEventArgs>((o, e) => { Console.WriteLine("--------- Msg received -----------\n" + e.Message.ToString()); });
            es.Start();
            Console.WriteLine("EventSource started");

            Console.ReadKey();
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
