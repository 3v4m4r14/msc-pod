using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using SimpleTCP;
using ElongatedBreathing.Properties;


namespace ElongatedBreathing
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        SimpleTcpClient Client;

        //Actuator parameters
        private static readonly int HEATER_INTENSITY_WHEN_TURNED_OFF = 0;
        private static readonly int HEATER_INTENSITY_WHEN_TURNED_ON = 1;
        private static readonly int FAN_INTENSITY_WHEN_TURNED_OFF = 0;
        private static readonly int FAN_INTENSITY_WHEN_TURNED_ON = 1;
        private static readonly int RED = 1;
        private static readonly int GREEN = 1;
        private static readonly int BLUE = 1;
        private static readonly int WHITE = 1;

        //Timing
        private static readonly int BREATHING_INTERVAL = 14000;
        private static readonly int ACTUATOR_INTERVAL = 6000;
        private static readonly int SLOW_HEARTBEAT_INTERVAL = 900;
        private static readonly int FAST_HEARTBEAT_INTERVAL = 700;
        private int heartbeatInterval = 900;

        private DateTime prevBreathTime = DateTime.Now;
        private DateTime prevHeartbeatTime = DateTime.Now;
        private DateTime curTime;

        //Audio
        MediaPlayer breathPlayer = new MediaPlayer();
        MediaPlayer slowHeartbeatPlayer = new MediaPlayer();
        MediaPlayer fastHeartbeatPlayer = new MediaPlayer();

        //Misc
        private static bool dynamicHeartbeat = true;
        private bool breathingIn = false;

        public App()
        {
            //Client = new SimpleTcpClient().Connect("127.0.0.1",3000);
            breathPlayer.Open(new Uri(@"C:\Users\eva\Desktop\msc-pod\ElongatedBreathing\ElongatedBreathing\audio\single-breath-long.wav"));
            slowHeartbeatPlayer.Open(new Uri(@"C:\Users\eva\Desktop\msc-pod\ElongatedBreathing\ElongatedBreathing\audio\bpm_0_70.wav"));
            fastHeartbeatPlayer.Open(new Uri(@"C:\Users\eva\Desktop\msc-pod\ElongatedBreathing\ElongatedBreathing\audio\bpm_0_100.wav"));

            Console.WriteLine("Hello, WOrld");

            while (true) { StartLoop(); }
        }

        private void StartLoop()
        {
            curTime = DateTime.Now;

            if (TimeToBreatheIn())
            {
                BreatheIn();
            }
            if (TimeToBreatheOut())
            {
                BreatheOut();
            }
            if (TimeForHeartbeat())
            {
                if (dynamicHeartbeat && breathingIn) { PlayFastHeartbeatAudio(); }
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

        private void BreatheOut()
        {
            Console.WriteLine("OFF");
            //TurnHeatOff();
            //TurnFanOff();
            //TurnLightOff();

            breathingIn = false;
        }

        private void BreatheIn()
        {
            breathPlayer.Stop();
            Console.WriteLine("ON");
            //TurnHeatOn();
            //TurnFanOn();
            //TurnLightOn();
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

        private void TurnHeatOn()
        {
            Client.WriteLine("SetHeaterIntensity|LEFT|" + HEATER_INTENSITY_WHEN_TURNED_ON);
            Client.WriteLine("SetHeaterIntensity|RIGHT|" + HEATER_INTENSITY_WHEN_TURNED_ON);
            Client.WriteLine("SetHeaterIntensity|SEAT_LEFT|" + HEATER_INTENSITY_WHEN_TURNED_ON);
            Client.WriteLine("SetHeaterIntensity|SEAT_RIGHT|" + HEATER_INTENSITY_WHEN_TURNED_ON);
            Client.WriteLine("SetHeaterIntensity|FRONT|" + HEATER_INTENSITY_WHEN_TURNED_ON);
        }

        private void TurnHeatOff()
        {
            Client.WriteLine("SetHeaterIntensity|LEFT|" + HEATER_INTENSITY_WHEN_TURNED_OFF);
            Client.WriteLine("SetHeaterIntensity|RIGHT|" + HEATER_INTENSITY_WHEN_TURNED_OFF);
            Client.WriteLine("SetHeaterIntensity|SEAT_LEFT|" + HEATER_INTENSITY_WHEN_TURNED_OFF);
            Client.WriteLine("SetHeaterIntensity|SEAT_RIGHT|" + HEATER_INTENSITY_WHEN_TURNED_OFF);
            Client.WriteLine("SetHeaterIntensity|FRONT|" + HEATER_INTENSITY_WHEN_TURNED_OFF);
        }

        private void TurnFanOn()
        {
            Client.WriteLine("SetFanIntensity|FRONT_LEFT|" + FAN_INTENSITY_WHEN_TURNED_ON);
            Client.WriteLine("SetFanIntensity|FRONT_RIGHT|" + FAN_INTENSITY_WHEN_TURNED_ON);
            Client.WriteLine("SetFanIntensity|REAR_LEFT|" + FAN_INTENSITY_WHEN_TURNED_ON);
            Client.WriteLine("SetFanIntensity|REAR_RIGHT|" + FAN_INTENSITY_WHEN_TURNED_ON);
        }

        private void TurnFanOff()
        {
            Client.WriteLine("SetFanIntensity|FRONT_LEFT|" + FAN_INTENSITY_WHEN_TURNED_OFF);
            Client.WriteLine("SetFanIntensity|FRONT_RIGHT|" + FAN_INTENSITY_WHEN_TURNED_OFF);
            Client.WriteLine("SetFanIntensity|REAR_LEFT|" + FAN_INTENSITY_WHEN_TURNED_OFF);
            Client.WriteLine("SetFanIntensity|REAR_RIGHT|" + FAN_INTENSITY_WHEN_TURNED_OFF);
        }

        private void TurnLightOn()
        {
            Client.WriteLine(String.Format("SetLightColor|LEFT|{0}|{1}|{2}|{3}", RED, GREEN, BLUE, WHITE));
            Client.WriteLine(String.Format("SetLightColor|LEFT|{0}|{1}|{2}|{3}", RED, GREEN, BLUE, WHITE));
        }

        private void TurnLightOff()
        {
            Client.WriteLine("SetLightColor|LEFT|0|0|0|0");
            Client.WriteLine("SetLightColor|RIGHT|0|0|0|0");
        }
    }
}
