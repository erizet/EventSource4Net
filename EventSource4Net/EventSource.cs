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
        private static readonly slf4net.ILogger _logger = slf4net.LoggerFactory.GetLogger(typeof(EventSource));

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
                    StringBuilder sb = new StringBuilder("State changed from ");
                    sb.Append(mCurrentState == null ? "Unknown" : mCurrentState.State.ToString());
                    sb.Append(" to ");
                    sb.Append(value == null ? "Unknown" : value.State.ToString());
                    _logger.Trace(sb.ToString());
                    mCurrentState = value;
                    OnStateChanged(mCurrentState.State);
                }
            }
        }

        public EventSource(Uri url)
        {
            Url = url;
            CurrentState = new DisconnectedState(Url);
            _logger.Info("EventSource created for " + url.ToString());
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
