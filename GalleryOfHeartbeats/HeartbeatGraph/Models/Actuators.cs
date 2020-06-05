using GalleryOfHeartbeats.Models;
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
        private Timer TimerForTurningOffHeat;

        private const int ACTUATOR_DURATION = 200;

        private float HEATER_INTENSITY_WHEN_IN = 0.7f;
        private float HEATER_INTENSITY_WHEN_OUT = 0f;
        private float FAN_INTENSITY_WHEN_IN = 0.5f;
        private float FAN_INTENSITY_WHEN_OUT = 0.2f;
        private const float RED = 0.3f;
        private const float GREEN = 0f;
        private const float BLUE = 0f;
        private const float WHITE = 0.1f;
        private bool WithLight = false;

        private bool HrIsIncreasing = false;
        private int PrevHr = 60;

        public Actuators()
        {
           // Client = new SimpleTcpClient().Connect("127.0.0.1", 3000);

            TimerForTurningOffActuators = new Timer();
            TimerForTurningOffActuators.Interval = ACTUATOR_DURATION;
            TimerForTurningOffActuators.Elapsed += new ElapsedEventHandler(TurnOffActuators);

            TimerForTurningOffHeat = new Timer();
            TimerForTurningOffHeat.Interval = ACTUATOR_DURATION;
            TimerForTurningOffHeat.Elapsed += new ElapsedEventHandler(TurnOffRecommendedActuators);

            //Client.WriteLine("SetActiveCeilingAnimation|OFF");
        }

        public Actuators(Settings settings) : this()
        {
            UpdateSettings(settings);
        }

        public void UpdateSettings(Settings settings)
        {
            HEATER_INTENSITY_WHEN_IN = settings.MaxHeatIntensity;
            HEATER_INTENSITY_WHEN_OUT = settings.MinHeatIntensity;
            FAN_INTENSITY_WHEN_IN = settings.MaxFanIntensity;
            FAN_INTENSITY_WHEN_OUT = settings.MinFanIntensity;
            WithLight = settings.LightOn;
        }

        public void ActivateWithHeartbeat()
        {
            Console.WriteLine("Actuators ON");
            //TurnOnActuators();
            StartActuatorTimer();
        }

        public void ActivateWhenHrIncreases(int current)
        {
            SetHrIsIncreasing(current);

            if (HrIsIncreasing)
            {
                Console.WriteLine("Act ON");
                //TurnOnActuators();
            }
            else
            {
                Console.WriteLine("Act OFF");
                //TurnOffActuators();
            }

        }

        private void SetHrIsIncreasing(int current)
        {
            if (PrevHr < current)
            {
                HrIsIncreasing = true;
                PrevHr = current;
            }
            else if (PrevHr > current)
            {
                HrIsIncreasing = false;
                PrevHr = current;
            }
        }

        public void RecommendedExperience(int current)
        {
            SetHrIsIncreasing(current);
            if (HrIsIncreasing)
            {
                Console.WriteLine("Rec air ON");
                //FanIn();
            }
            else
            {
                Console.WriteLine("Rec air OFF");
                //FanOut();
            }

            Console.WriteLine("Rec heat ON");
            //HeatIn();
            TimerForTurningOffHeat.Start();
        }

        public void ResetActuators()
        {
            Console.WriteLine("Actuators reset.");
            //TurnOffActuators();
        }

        private void StartActuatorTimer()
        {
            TimerForTurningOffActuators.Start();
        }

        private void TurnOnActuators()
        {
            HeatIn();
            FanIn();
            if (WithLight)
            {
                LightIn();
            }

        }
        private void TurnOffActuators(object sender, ElapsedEventArgs e)
        {
            //TurnOffActuators();
            TimerForTurningOffActuators.Stop();
        }
        private void TurnOffRecommendedActuators(object sender, ElapsedEventArgs e)
        {
            //HeatOut();
            TimerForTurningOffHeat.Stop();
        }
        private void TurnOffActuators()
        {
            Console.WriteLine("Actuators OFF");
            HeatOut();
            FanOut();
            if (WithLight)
            {
                LightOut();
            }

        }

        private void HeatOut()
        {
            Client.WriteLine("SetHeaterIntensity|LEFT|" + HEATER_INTENSITY_WHEN_OUT);
            Client.WriteLine("SetHeaterIntensity|RIGHT|" + HEATER_INTENSITY_WHEN_OUT);
            Client.WriteLine("SetHeaterIntensity|SEAT_LEFT|" + HEATER_INTENSITY_WHEN_OUT);
            Client.WriteLine("SetHeaterIntensity|SEAT_RIGHT|" + HEATER_INTENSITY_WHEN_OUT);
            Client.WriteLine("SetHeaterIntensity|FRONT|" + HEATER_INTENSITY_WHEN_OUT);
        }

        private void HeatIn()
        {
            Client.WriteLine("SetHeaterIntensity|LEFT|" + HEATER_INTENSITY_WHEN_IN);
            Client.WriteLine("SetHeaterIntensity|RIGHT|" + HEATER_INTENSITY_WHEN_IN);
            Client.WriteLine("SetHeaterIntensity|SEAT_LEFT|" + HEATER_INTENSITY_WHEN_IN);
            Client.WriteLine("SetHeaterIntensity|SEAT_RIGHT|" + HEATER_INTENSITY_WHEN_IN);
            Client.WriteLine("SetHeaterIntensity|FRONT|" + HEATER_INTENSITY_WHEN_IN);
        }

        private void FanIn()
        {
            Client.WriteLine("SetFanIntensity|FRONT_LEFT|" + FAN_INTENSITY_WHEN_IN);
            Client.WriteLine("SetFanIntensity|FRONT_RIGHT|" + FAN_INTENSITY_WHEN_IN);
            Client.WriteLine("SetFanIntensity|REAR_LEFT|" + FAN_INTENSITY_WHEN_IN);
            Client.WriteLine("SetFanIntensity|REAR_RIGHT|" + FAN_INTENSITY_WHEN_IN);
        }

        private void FanOut()
        {
            Client.WriteLine("SetFanIntensity|FRONT_LEFT|" + FAN_INTENSITY_WHEN_OUT);
            Client.WriteLine("SetFanIntensity|FRONT_RIGHT|" + FAN_INTENSITY_WHEN_OUT);
            Client.WriteLine("SetFanIntensity|REAR_LEFT|" + FAN_INTENSITY_WHEN_OUT);
            Client.WriteLine("SetFanIntensity|REAR_RIGHT|" + FAN_INTENSITY_WHEN_OUT);
        }

        private void LightIn()
        {
            Client.WriteLine(String.Format("SetLightColor|LEFT|{0}|{1}|{2}|{3}", RED, GREEN, BLUE, WHITE));
            Client.WriteLine(String.Format("SetLightColor|RIGHT|{0}|{1}|{2}|{3}", RED, GREEN, BLUE, WHITE));
        }

        private void LightOut()
        {
            Client.WriteLine("SetLightColor|LEFT|0|0|0|0");
            Client.WriteLine("SetLightColor|RIGHT|0|0|0|0");
        }
    }
}
