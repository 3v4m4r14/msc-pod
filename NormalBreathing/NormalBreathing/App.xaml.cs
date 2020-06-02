using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using SimpleTCP;
using NormalBreathing.Properties;
using System.IO;
using System.Reflection;

namespace NormalBreathing
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        SimpleTcpClient Client;

        //Actuator parameters
        private static readonly float HEATER_INTENSITY_WHEN_EXHALING = 0.2f;
        private static readonly float HEATER_INTENSITY_WHEN_INHALING = 0f;
        private static readonly float FAN_INTENSITY_WHEN_EXHALING = 0f;
        private static readonly float FAN_INTENSITY_WHEN_INHALING = 0.5f;
        private static readonly int RED = 1;
        private static readonly int GREEN = 1;
        private static readonly int BLUE = 1;
        private static readonly int WHITE = 1;

        //Timing
        private static readonly int BREATHING_INTERVAL = 4000;
        private static readonly int ACTUATOR_INTERVAL = 1500;
        private static readonly int SLOW_HEARTBEAT_INTERVAL = 900;
        private static readonly int FAST_HEARTBEAT_INTERVAL = 600;
        private int heartbeatInterval = SLOW_HEARTBEAT_INTERVAL;

        private DateTime prevBreathTime = DateTime.Now;
        private DateTime prevHeartbeatTime = DateTime.Now;
        private DateTime curTime;

        //Audio
        private readonly MediaPlayer breathPlayer = new MediaPlayer();
        private readonly MediaPlayer slowHeartbeatPlayer = new MediaPlayer();
        private readonly MediaPlayer fastHeartbeatPlayer = new MediaPlayer();

        //Misc
        private static readonly bool WITH_HEARTBEAT = true;
        private static readonly bool DYNAMIC_HEARTBEAT = false;
        private bool breathingIn = false;

        public App()
        {
            Client = new SimpleTcpClient().Connect("127.0.0.1",3000);
            String audioPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "audio");

            breathPlayer.Open(new Uri(Path.Combine(audioPath, "single-breath-normal.wav")));
            slowHeartbeatPlayer.Open(new Uri(Path.Combine(audioPath, "bpm_0_70.wav")));
            fastHeartbeatPlayer.Open(new Uri(Path.Combine(audioPath, "bpm_0_100.wav")));

            Client.WriteLine("SetActiveCeilingAnimation|OFF");

            Console.WriteLine("Hello, WOrld");

            while (true) { StartLoop(); }
        }

        private void StartLoop()
        {
            curTime = DateTime.Now;

            if (TimeToBreatheIn())
            {
                Inhaling();
            }
            if (TimeToBreatheOut())
            {
                Exhaling();
            }
            if (WITH_HEARTBEAT && TimeForHeartbeat())
            {
                if (DYNAMIC_HEARTBEAT && breathingIn) { PlayFastHeartbeatAudio(); }
                else { PlaySlowHeartbeatAudio(); }
            }
        }

        private void PlaySlowHeartbeatAudio()
        {
            slowHeartbeatPlayer.Stop();
            slowHeartbeatPlayer.Play();

            heartbeatInterval = SLOW_HEARTBEAT_INTERVAL;
            prevHeartbeatTime = curTime;
        }

        private void PlayFastHeartbeatAudio()
        {
            fastHeartbeatPlayer.Stop();
            fastHeartbeatPlayer.Play();

            heartbeatInterval = FAST_HEARTBEAT_INTERVAL;
            prevHeartbeatTime = curTime;
        }

        private void Exhaling()
        {
            Console.WriteLine("OFF");
            HeatExhale();
            FanExhale();
            //LightExhale();

            breathingIn = false;
        }

        private void Inhaling()
        {
            breathPlayer.Stop();
            Console.WriteLine("ON");
            HeatInhale();
            FanInhale();
            //LightInhale();


            breathingIn = true;

            breathPlayer.Play();

            prevBreathTime = curTime;
        }

        private bool TimeForHeartbeat()
        {
            return (curTime - prevHeartbeatTime).TotalMilliseconds > heartbeatInterval;
        }

        private bool TimeToBreatheOut()
        {
            return breathingIn && (curTime - prevBreathTime).TotalMilliseconds > ACTUATOR_INTERVAL;
        }

        private bool TimeToBreatheIn()
        {
            return (curTime - prevBreathTime).TotalMilliseconds > BREATHING_INTERVAL;
        }

        private void HeatInhale()
        {
            Client.WriteLine("SetHeaterIntensity|LEFT|" + HEATER_INTENSITY_WHEN_INHALING);
            Client.WriteLine("SetHeaterIntensity|RIGHT|" + HEATER_INTENSITY_WHEN_INHALING);
            Client.WriteLine("SetHeaterIntensity|SEAT_LEFT|" + HEATER_INTENSITY_WHEN_INHALING);
            Client.WriteLine("SetHeaterIntensity|SEAT_RIGHT|" + HEATER_INTENSITY_WHEN_INHALING);
            Client.WriteLine("SetHeaterIntensity|FRONT|" + HEATER_INTENSITY_WHEN_INHALING);
        }

        private void HeatExhale()
        {
            Client.WriteLine("SetHeaterIntensity|LEFT|" + HEATER_INTENSITY_WHEN_EXHALING);
            Client.WriteLine("SetHeaterIntensity|RIGHT|" + HEATER_INTENSITY_WHEN_EXHALING);
            Client.WriteLine("SetHeaterIntensity|SEAT_LEFT|" + HEATER_INTENSITY_WHEN_EXHALING);
            Client.WriteLine("SetHeaterIntensity|SEAT_RIGHT|" + HEATER_INTENSITY_WHEN_EXHALING);
            Client.WriteLine("SetHeaterIntensity|FRONT|" + HEATER_INTENSITY_WHEN_EXHALING);
        }

        private void FanInhale()
        {
            Client.WriteLine("SetFanIntensity|FRONT_LEFT|" + FAN_INTENSITY_WHEN_INHALING);
            Client.WriteLine("SetFanIntensity|FRONT_RIGHT|" + FAN_INTENSITY_WHEN_INHALING);
            Client.WriteLine("SetFanIntensity|REAR_LEFT|" + FAN_INTENSITY_WHEN_INHALING);
            Client.WriteLine("SetFanIntensity|REAR_RIGHT|" + FAN_INTENSITY_WHEN_INHALING);
        }

        private void FanExhale()
        {
            Client.WriteLine("SetFanIntensity|FRONT_LEFT|" + FAN_INTENSITY_WHEN_EXHALING);
            Client.WriteLine("SetFanIntensity|FRONT_RIGHT|" + FAN_INTENSITY_WHEN_EXHALING);
            Client.WriteLine("SetFanIntensity|REAR_LEFT|" + FAN_INTENSITY_WHEN_EXHALING);
            Client.WriteLine("SetFanIntensity|REAR_RIGHT|" + FAN_INTENSITY_WHEN_EXHALING);
        }

        private void LightInhale()
        {
            Client.WriteLine(String.Format("SetLightColor|LEFT|{0}|{1}|{2}|{3}", RED, GREEN, BLUE, WHITE));
            Client.WriteLine(String.Format("SetLightColor|LEFT|{0}|{1}|{2}|{3}", RED, GREEN, BLUE, WHITE));
        }

        private void LightExhale()
        {
            Client.WriteLine("SetLightColor|LEFT|0|0|0|0");
            Client.WriteLine("SetLightColor|RIGHT|0|0|0|0");
        }
    }
}
