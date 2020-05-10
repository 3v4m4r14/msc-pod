using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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


        System.Media.SoundPlayer player;

        private DateTime prevTimestamp = DateTime.Now;
        private DateTime curTimestamp;
        private TimeSpan breathingInterval = new TimeSpan(0, 0, 14);
        private TimeSpan actuatorInterval = new TimeSpan(0, 0, 6);

        private bool actuatorIsOn = false;

        public App()
        {
            //Client = new SimpleTcpClient().Connect("127.0.0.1",3000);
            player = new System.Media.SoundPlayer(ElongatedBreathing.Properties.Resources.singleBreathLong);


            Console.WriteLine("Hello, WOrld");

            while (true) { StartLoop(); }
        }

        private void StartLoop()
        {
            curTimestamp = DateTime.Now;
            if (curTimestamp - prevTimestamp > breathingInterval)
            {

                Console.WriteLine("ON");
                //TurnHeatOn();
                //TurnFanOn();
                //TurnLightOn();
                actuatorIsOn = true;

                player.Play();

                prevTimestamp = curTimestamp;
            }
            if (actuatorIsOn && curTimestamp - prevTimestamp > actuatorInterval)
            {
                Console.WriteLine("OFF");
                //TurnHeatOff();
                //TurnFanOff();
                //TurnLightOff();

                actuatorIsOn = false;
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
