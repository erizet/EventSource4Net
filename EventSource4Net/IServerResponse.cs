using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace EventSource4Net
{
    public interface IServerResponse
    {
        HttpStatusCode StatusCode { get; }

        System.IO.Stream GetResponseStream();

        Uri ResponseUri { get; }
    }
}
