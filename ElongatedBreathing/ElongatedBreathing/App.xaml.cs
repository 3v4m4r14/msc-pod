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
        private static int heaterIntensityWhenTurnedOff = 0;
        private static int heaterIntensityWhenTurnedOn = 1;
        private static int fanIntensityWhenTurnedOff = 0;
        private static int fanIntensityWhenTurnedOn = 1;
        private int r = 1;
        private int g = 1;
        private int b = 1;
        private int w = 1;


        MediaPlayer breathPlayer = new MediaPlayer();
        MediaPlayer steadyHeartbeatPlayer = new MediaPlayer();

        private DateTime prevBreathTime = DateTime.Now;
        private DateTime curTime;
        private DateTime prevHeartbeatTime = DateTime.Now;
        private DateTime curHeartbeatTime;
        private int breathingInterval = 14000;
        private int actuatorInterval = 6000;
        private int heartbeatInterval = 900;

        private bool actuatorIsOn = false;
        
        public App()
        {
            //Client = new SimpleTcpClient().Connect("127.0.0.1",3000);
            breathPlayer.Open(new System.Uri(@"C:\Users\eva\Desktop\msc-pod\ElongatedBreathing\ElongatedBreathing\audio\single-breath-long.wav"));
            steadyHeartbeatPlayer.Open(new System.Uri(@"C:\Users\eva\Desktop\msc-pod\ElongatedBreathing\ElongatedBreathing\audio\bpm_0_70.wav"));

            Console.WriteLine("Hello, WOrld");

            while (true) { StartLoop(); }
        }

        private void StartLoop()
        {
            curTime = DateTime.Now;

            if ((curTime - prevBreathTime).TotalMilliseconds > breathingInterval)
            {
                breathPlayer.Stop();
                Console.WriteLine("ON");
                //TurnHeatOn();
                //TurnFanOn();
                //TurnLightOn();
                actuatorIsOn = true;

                breathPlayer.Play();

                prevBreathTime = curTime;
            }
            if (actuatorIsOn && (curTime - prevBreathTime).TotalMilliseconds > actuatorInterval)
            {
                Console.WriteLine("OFF");
                //TurnHeatOff();
                //TurnFanOff();
                //TurnLightOff();

                actuatorIsOn = false;
            }
            if ((curTime - prevHeartbeatTime).TotalMilliseconds > heartbeatInterval)
            {
                steadyHeartbeatPlayer.Stop();
                steadyHeartbeatPlayer.Play();

                prevHeartbeatTime = curTime;
            }
        }

        private void TurnHeatOn()
        {
            Client.WriteLine("SetHeaterIntensity|LEFT|" + heaterIntensityWhenTurnedOn);
            Client.WriteLine("SetHeaterIntensity|RIGHT|" + heaterIntensityWhenTurnedOn);
            Client.WriteLine("SetHeaterIntensity|SEAT_LEFT|" + heaterIntensityWhenTurnedOn);
            Client.WriteLine("SetHeaterIntensity|SEAT_RIGHT|" + heaterIntensityWhenTurnedOn);
            Client.WriteLine("SetHeaterIntensity|FRONT|" + heaterIntensityWhenTurnedOn);
        }

        private void TurnHeatOff()
        {
            Client.WriteLine("SetHeaterIntensity|LEFT|" + heaterIntensityWhenTurnedOff);
            Client.WriteLine("SetHeaterIntensity|RIGHT|" + heaterIntensityWhenTurnedOff);
            Client.WriteLine("SetHeaterIntensity|SEAT_LEFT|" + heaterIntensityWhenTurnedOff);
            Client.WriteLine("SetHeaterIntensity|SEAT_RIGHT|" + heaterIntensityWhenTurnedOff);
            Client.WriteLine("SetHeaterIntensity|FRONT|" + heaterIntensityWhenTurnedOff);
        }

        private void TurnFanOn()
        {
            Client.WriteLine("SetFanIntensity|FRONT_LEFT|" + fanIntensityWhenTurnedOn);
            Client.WriteLine("SetFanIntensity|FRONT_RIGHT|" + fanIntensityWhenTurnedOn);
            Client.WriteLine("SetFanIntensity|REAR_LEFT|" + fanIntensityWhenTurnedOn);
            Client.WriteLine("SetFanIntensity|REAR_RIGHT|" + fanIntensityWhenTurnedOn);
        }

        private void TurnFanOff()
        {
            Client.WriteLine("SetFanIntensity|FRONT_LEFT|" + fanIntensityWhenTurnedOff);
            Client.WriteLine("SetFanIntensity|FRONT_RIGHT|" + fanIntensityWhenTurnedOff);
            Client.WriteLine("SetFanIntensity|REAR_LEFT|" + fanIntensityWhenTurnedOff);
            Client.WriteLine("SetFanIntensity|REAR_RIGHT|" + fanIntensityWhenTurnedOff);
        }

        private void TurnLightOn()
        {
            Client.WriteLine(String.Format("SetLightColor|LEFT|{0}|{1}|{2}|{3}", r, g, b, w));
            Client.WriteLine(String.Format("SetLightColor|LEFT|{0}|{1}|{2}|{3}", r, g, b, w));
        }

        private void TurnLightOff()
        {
            Client.WriteLine("SetLightColor|LEFT|0|0|0|0");
            Client.WriteLine("SetLightColor|RIGHT|0|0|0|0");
        }
    }
}
