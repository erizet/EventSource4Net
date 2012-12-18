using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace EventSource4Net
{
    public class EventSource
    {
        public event EventHandler<StateChangedEventArgs> StateChanged;
        public event EventHandler<ServerSentEventReceivedEventArgs> EventReceived;

        public Uri Url { get; private set; }
        public EventSourceState State { get { return CurrentState.State; } }
        public string LastEventId { get; private set; }
        private IConnectionState mCurrentState = null;
        private IConnectionState CurrentState
        {
            get { return mCurrentState; }
            set
            {
                if (!value.Equals(mCurrentState))
                {
                    mCurrentState = value;
                    OnStateChanged(mCurrentState.State);
                }
            }
        }

        public EventSource(Uri url)
        {
            Url = url;
            CurrentState = new DisconnectedState(Url);
        }

        public void Start()
        {
            if (State == EventSourceState.CLOSED)
            {
                Run();
            }
        }

        public void Stop()
        {
        }

        protected void Run()
        {
            mCurrentState.Run(this.OnEventReceived).ContinueWith(cs =>
            {
                CurrentState = cs.Result;
                Run();
            });
        }

        protected void OnEventReceived(ServerSentEvent sse)
        {
            if (EventReceived != null)
            {
                EventReceived(this, new ServerSentEventReceivedEventArgs(sse));
            }
        }

        protected void OnStateChanged(EventSourceState newState)
        {
            if (StateChanged != null)
            {
                StateChanged(this, new StateChangedEventArgs(newState));
            }
        }
    }
}
