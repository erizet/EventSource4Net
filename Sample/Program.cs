using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSource4Net;


namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            EventSource es = new EventSource(new Uri(@"http://localhost:49999/api/chart/"));
            es.StateChanged += new EventHandler<StateChangedEventArgs>((o, e) => { Console.WriteLine("New state: " + e.State.ToString()); });
            es.EventReceived += new EventHandler<ServerSentEventReceivedEventArgs>((o, e) => { Console.WriteLine("--------- Msg received -----------\n" + e.Message.ToString()); });
            es.Start();
            Console.WriteLine("EventSource started");

            Console.ReadKey();
        }
    }
}
