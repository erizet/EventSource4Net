using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EventSource4Net
{
    public interface IWebRequester
    {
        Task<IServerResponse> Get(Uri url);

    }
}
