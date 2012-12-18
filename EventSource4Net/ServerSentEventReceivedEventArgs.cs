using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventSource4Net
{
    public class ServerSentEventReceivedEventArgs : EventArgs
    {
        public ServerSentEvent Message { get; private set; }
        public ServerSentEventReceivedEventArgs(ServerSentEvent message)
        {
            Message = message;
        }

    }
}
