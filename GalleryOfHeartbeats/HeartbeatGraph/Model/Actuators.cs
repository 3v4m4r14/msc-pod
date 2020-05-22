using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace GalleryOfHeartbeats.Model
{
    class Actuators
    {
        private SimpleTcpClient Client;
        private Timer TimerForTurningOffActuators;

        private const int ACTUATOR_DURATION = 500;

        private const float HEATER_INTENSITY_WHEN_INHALING = 0;
        private const float HEATER_INTENSITY_WHEN_EXHALING = 0.2f;
        private const float FAN_INTENSITY_WHEN_INHALING = 0.5f;
        private const float FAN_INTENSITY_WHEN_EXHALING = 0f;
        private const float RED = 0.3f;
        private const float GREEN = 0f;
        private const float BLUE = 0f;
        private const float WHITE = 0.1f;

        public Actuators()
        {
            //Client = new SimpleTcpClient().Connect("127.0.0.1", 3000);
            TimerForTurningOffActuators = new Timer();
            TimerForTurningOffActuators.Interval = ACTUATOR_DURATION;
            TimerForTurningOffActuators.Elapsed += new ElapsedEventHandler(TurnOffActuators);
        }

        private void TurnOffActuators(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Actuators OFF");
            TimerForTurningOffActuators.Stop();
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

        public void OnHeartrateChangeAdvanced()
        {
            Console.WriteLine("Actuators ON");
            StartActuatorTimer();
        }

        private void StartActuatorTimer()
        {
            TimerForTurningOffActuators.Start();
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
