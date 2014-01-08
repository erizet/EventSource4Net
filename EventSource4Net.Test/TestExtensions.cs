using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventSource4Net.Test
{
    static class TestExtensions
    {
        public static void WaitOrThrow(this ManualResetEvent mre)
        {
#if DEBUG
            mre.WaitOne();
#else
            if (!mre.WaitOne(1000))
                throw new TimeoutException("Timeout waiting for manualresetevent");
#endif
        }
    }
}
