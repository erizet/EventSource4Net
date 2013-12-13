using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventSource4Net
{
    class DisconnectedState : IConnectionState 
    {
        private Uri mUrl;
        public EventSourceState State
        {
            get { return EventSourceState.CLOSED; }
        }

        public DisconnectedState(Uri url)
        {
            if (url == null) throw new ArgumentNullException("Url cant be null");
            mUrl = url;
        }

        public Task<IConnectionState> Run(Action<ServerSentEvent> donothing, CancellationToken cancelToken)
        {
            if(cancelToken.IsCancellationRequested)
                return Task.Factory.StartNew<IConnectionState>(() => { return new DisconnectedState(mUrl); });
            else
                return Task.Factory.StartNew<IConnectionState>(() => { return new ConnectingState(mUrl); });
        }
    }
}
