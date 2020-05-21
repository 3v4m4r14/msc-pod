using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GalleryOfHeartbeats.Model
{
    class Actuators
    {
        SimpleTcpClient Client;

        private static readonly float HEATER_INTENSITY_WHEN_INHALING = 0;
        private static readonly float HEATER_INTENSITY_WHEN_EXHALING = 0.2f;
        private static readonly float FAN_INTENSITY_WHEN_INHALING = 0.5f;
        private static readonly float FAN_INTENSITY_WHEN_EXHALING = 0f;
        private static readonly float RED = 0.3f;
        private static readonly float GREEN = 0f;
        private static readonly float BLUE = 0f;
        private static readonly float WHITE = 0.1f;

        

        public Actuators()
        {
            //Client = new SimpleTcpClient().Connect("127.0.0.1", 3000);
        }

        public void OnHeartrateChangeBasic(int previous, int current)
        {
            Console.WriteLine("Timer event called in Actuators class: " + previous + " " + current);

            if (previous < current)
            {
                Console.WriteLine("HR increased");
            }
            else
            {
                Console.WriteLine("HR decreased");
            }
        }

        public void OnHeartrateChangeAdvanced(int previous, int current)
        {

        }

        private void HeatExhale()
        {
            Client.WriteLine("SetHeaterIntensity|LEFT|" + HEATER_INTENSITY_WHEN_EXHALING);
            Client.WriteLine("SetHeaterIntensity|RIGHT|" + HEATER_INTENSITY_WHEN_EXHALING);
            Client.WriteLine("SetHeaterIntensity|SEAT_LEFT|" + HEATER_INTENSITY_WHEN_EXHALING);
            Client.WriteLine("SetHeaterIntensity|SEAT_RIGHT|" + HEATER_INTENSITY_WHEN_EXHALING);
            Client.WriteLine("SetHeaterIntensity|FRONT|" + HEATER_INTENSITY_WHEN_EXHALING);
        }

        private void HeatInhale()
        {
            Client.WriteLine("SetHeaterIntensity|LEFT|" + HEATER_INTENSITY_WHEN_INHALING);
            Client.WriteLine("SetHeaterIntensity|RIGHT|" + HEATER_INTENSITY_WHEN_INHALING);
            Client.WriteLine("SetHeaterIntensity|SEAT_LEFT|" + HEATER_INTENSITY_WHEN_INHALING);
            Client.WriteLine("SetHeaterIntensity|SEAT_RIGHT|" + HEATER_INTENSITY_WHEN_INHALING);
            Client.WriteLine("SetHeaterIntensity|FRONT|" + HEATER_INTENSITY_WHEN_INHALING);
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
            Client.WriteLine(String.Format("SetLightColor|RIGHT|{0}|{1}|{2}|{3}", RED, GREEN, BLUE, WHITE));
        }

        private void LightExhale()
        {
            Client.WriteLine("SetLightColor|LEFT|0|0|0|0");
            Client.WriteLine("SetLightColor|RIGHT|0|0|0|0");
        }
    }
}
