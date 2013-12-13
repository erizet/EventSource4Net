using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventSource4Net
{
    interface IConnectionState
    {
        EventSourceState State { get; }
        Task<IConnectionState> Run(Action<ServerSentEvent> MsgReceivedCallback, CancellationToken cancelToken);
    }
}
