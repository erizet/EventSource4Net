using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventSource4Net
{
    public class ServerSentEvent
    {
        public string LastEventId { get; set; }
        public string EventType { get; set; }
        public string Data { get; set; }
        public int? Retry { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("EventType: ").Append(EventType).AppendLine();
            sb.Append("Data: ").Append(Data).AppendLine();
            sb.Append("LastEventId: ").Append(LastEventId).AppendLine();
            if(Retry.HasValue)
                sb.Append("Retry: ").Append(Retry.Value).AppendLine();
            return sb.ToString();
        }
    }
}
