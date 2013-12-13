using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventSource4Net
{
    internal class Watchdog
    {
        private long _timeout;
        private Timer _timer;
        public event EventHandler TimerExpired;

        public void Start()
        {
            _timer = new Timer(new TimerCallback(OnTimerExpired), null, 0, _timeout);
        }

        public void Reset()
        {
            _timer.Change(0, _timeout);
        }

        private void OnTimerExpired(object State)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            if (TimerExpired != null)
                TimerExpired(this, new EventArgs());
        }

        public Watchdog(long timeout)
        {
            if (timeout < 1) throw new ArgumentOutOfRangeException("timeout", "timeout muste be greater than zero.");
        }
    }
}
