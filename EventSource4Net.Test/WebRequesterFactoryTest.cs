using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventSource4Net.Test
{
    class WebRequesterFactoryMock : IWebRequesterFactory
    {
        public WebRequesterMock WebRequesterMock
        {
            get;
            private set;
        }
        public WebRequesterFactoryMock(ServiceResponseMock response)
        {
             this.WebRequesterMock = new WebRequesterMock(response);
        }
        public IWebRequester Create()
        {
            return WebRequesterMock;
        }
    }

    class WebRequesterMock : IWebRequester
    {
        public ManualResetEvent GetCalled = new ManualResetEvent(false);
        public ServiceResponseMock Response { get; private set; }

        public WebRequesterMock(ServiceResponseMock response)
        {
            this.Response = response;
        }

        public System.Threading.Tasks.Task<IServerResponse> Get(Uri url)
        {
            return Task.Factory.StartNew<IServerResponse>(() =>
            {
                GetCalled.Set();
                return Response;
            });
        }
    }

    class ServiceResponseMock : IServerResponse
    {
        private MemoryStream mStream = new MemoryStream();
        private StreamWriter mStreamWriter;
        private Uri mUrl;
        private HttpStatusCode mStatusCode;

        public ManualResetEvent StatusCodeCalled = new ManualResetEvent(false);

        public ServiceResponseMock(Uri url, HttpStatusCode statusCode)
        {
            mUrl = url;
            mStatusCode = statusCode;
            mStreamWriter = new StreamWriter(mStream);
        }

        public System.Net.HttpStatusCode StatusCode
        {
            get
            {
                StatusCodeCalled.Set();
                return mStatusCode;
            }
        }

        public System.IO.Stream GetResponseStream()
        {
            return mStream;
        }

        public Uri ResponseUri
        {
            get { return mUrl; }
        }

        public void WriteTestTextToStream(string text)
        {
            mStreamWriter.Write(text);
        }
    }

    class GetIsCalledEventArgs : EventArgs
    {
        public ServiceResponseMock ServerResponse { get; private set; }
        public GetIsCalledEventArgs(ServiceResponseMock response)
        {
            ServerResponse = response;
        }
    }
}
