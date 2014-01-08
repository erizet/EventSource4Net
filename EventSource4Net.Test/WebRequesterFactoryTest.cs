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
        private Stream mStream;
        private StreamWriter mStreamWriter;
        private Uri mUrl;
        private HttpStatusCode mStatusCode;

        public ManualResetEvent StatusCodeCalled = new ManualResetEvent(false);

        public ServiceResponseMock(Uri url, HttpStatusCode statusCode)
        {
            mUrl = url;
            mStatusCode = statusCode;
            mStream = new TestableStream();
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
            mStreamWriter.Flush();
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


    class TestableStream : Stream
    {
        long _pos = 0;
        System.Collections.Concurrent.BlockingCollection<string> _texts = new System.Collections.Concurrent.BlockingCollection<string>();

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {

        }

        public override long Length
        {
            get { return _texts.Count(); }
        }

        public override long Position
        {
            get
            {
                return _pos;
            }
            set
            {
                _pos = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            string s = _texts.Take();

            byte[] encodedText = Encoding.UTF8.GetBytes(s);
            encodedText.CopyTo(buffer, offset);
            return encodedText.Length;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            string s = Encoding.UTF8.GetString(buffer, offset, count);
            _texts.Add(s);
            //_texts.CompleteAdding();
        }
    }


}
