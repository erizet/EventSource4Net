using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource4Net.Test
{
    class TestableEventSource : EventSource
    {
        public TestableEventSource(Uri url,IWebRequesterFactory factory) : base(url,factory)
        {

        }
    }
}
