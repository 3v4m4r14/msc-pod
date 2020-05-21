using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace GalleryOfHeartbeats.Model
{
    class AudioPlayer
    {
        private const int ONE_MINUTE = 60000;
        private static readonly int SLOW_HEARTBEAT_INTERVAL = 900;
        private static readonly int FAST_HEARTBEAT_INTERVAL = 700;
        private int heartbeatInterval = SLOW_HEARTBEAT_INTERVAL;

        private readonly MediaPlayer slowHeartbeatPlayer = new MediaPlayer();
        private readonly MediaPlayer fastHeartbeatPlayer = new MediaPlayer();

        private DateTime prevHeartbeatTime = DateTime.Now;
        private DateTime curTime;

        public AudioPlayer()
        {
            string audioPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "audio");

            slowHeartbeatPlayer.Open(new Uri(Path.Combine(audioPath, "bpm_0_70.wav")));
            fastHeartbeatPlayer.Open(new Uri(Path.Combine(audioPath, "bpm_0_100.wav")));
        }

        public void PlayHeartbeatSound(int heartrate)
        {
            //http://www.sengpielaudio.com/calculator-bpmtempotime.htm

            double _temp = ONE_MINUTE / heartrate;
            heartbeatInterval = (int) Math.Ceiling(_temp);

            Console.WriteLine("Interval: " + heartbeatInterval);

            curTime = DateTime.Now;

            if (TimeForHeartbeat())
            {
                PlaySlowHeartbeatAudio();
            }
        }

        private bool TimeForHeartbeat()
        {
            return (curTime - prevHeartbeatTime).TotalMilliseconds > heartbeatInterval;
        }

        private void PlaySlowHeartbeatAudio()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                slowHeartbeatPlayer.Stop();
                slowHeartbeatPlayer.Play();
            }));
                
            prevHeartbeatTime = curTime;
        }
    }
}
