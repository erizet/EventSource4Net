using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace EventSource4Net
{
    class ConnectingState : IConnectionState
    {
        private Uri mUrl;
        public EventSourceState State { get { return EventSourceState.CONNECTING; } }
        
        public ConnectingState(Uri url)
        {
            if(url==null) throw new ArgumentNullException("Url cant be null");
            mUrl = url;
        }
        
        public Task<IConnectionState> Run(Action<ServerSentEvent> donothing)
        {
            var wreq = (HttpWebRequest)WebRequest.Create(mUrl);
            wreq.Method = "GET";
            wreq.Proxy = null;

            var taskResp = Task.Factory.FromAsync<WebResponse>(wreq.BeginGetResponse,
                                                            wreq.EndGetResponse,
                                                            null);

            return taskResp.ContinueWith<IConnectionState>(tsk => 
            {
                if (tsk.Status == TaskStatus.RanToCompletion)
                {
                    HttpWebResponse resp = tsk.Result as HttpWebResponse;
                    if (resp != null && resp.StatusCode == HttpStatusCode.OK)
                    {
                        return new ConnectedState(resp);
                    }
                }

                return new DisconnectedState(mUrl);
            });
        }
    }
}
