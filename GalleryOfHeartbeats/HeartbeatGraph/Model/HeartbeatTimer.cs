using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryOfHeartbeats.Model
{
    class HeartbeatTimer
    {
        private const int ONE_MINUTE = 60000;
        private const int SLOW_HEARTBEAT_INTERVAL = 900;
        private const int FAST_HEARTBEAT_INTERVAL = 700;
        private int heartbeatInterval = 1000;

        private DateTime prevHeartbeatTime = DateTime.Now;
        private DateTime curTime;

        public HeartbeatTimer() { }

        private bool IntervalHasPassed()
        {
            return (curTime - prevHeartbeatTime).TotalMilliseconds >= heartbeatInterval;
        }

        public bool TimeForHeartbeat(int heartrate)
        {
            curTime = DateTime.Now;

            if (heartrate == 0) { heartrate = 60; }

            heartbeatInterval = (int)Math.Ceiling((double) ONE_MINUTE / heartrate);


            if (IntervalHasPassed())
            {
                Console.WriteLine("Interval has passed: " + heartbeatInterval + " " + (curTime - prevHeartbeatTime).TotalMilliseconds);
                prevHeartbeatTime = DateTime.Now;
                return true;
            }
            return false;
        }
    }
}
