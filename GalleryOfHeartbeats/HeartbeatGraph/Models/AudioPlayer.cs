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
        private readonly MediaPlayer slowHeartbeatPlayer = new MediaPlayer();
        private readonly MediaPlayer fastHeartbeatPlayer = new MediaPlayer();

        public AudioPlayer()
        {
            string audioPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "audio");

            slowHeartbeatPlayer.Open(new Uri(Path.Combine(audioPath, "bpm_80.wav")));
            fastHeartbeatPlayer.Open(new Uri(Path.Combine(audioPath, "bpm_0_100.wav")));
        }

        public void PlayHeartbeatAudio()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                slowHeartbeatPlayer.Stop();
                slowHeartbeatPlayer.Play();
            }));
        }
    }
}
