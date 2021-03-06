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

        private const int ACTUATOR_DURATION = 300;
        private const int NIL_POWER = 0;

        private float HEATER_INTENSITY_WHEN_IN = 0.7f;
        private float HEATER_INTENSITY_WHEN_OUT = 0f;
        private float FAN_INTENSITY_WHEN_IN = 0.5f;
        private float FAN_INTENSITY_WHEN_OUT = 0.2f;
        private const float RED = 0.7f;
        private const float GREEN = 0f;
        private const float BLUE = 0f;
        private const float WHITE = 0.1f;
        private bool WithLight = false;

        private bool HrIsIncreasing = false;
        private int PrevHr = 60;

        public Actuators()
        {
            Client = new SimpleTcpClient().Connect("127.0.0.1", 3000);

            TimerForTurningOffActuators = new Timer();
            TimerForTurningOffActuators.Interval = ACTUATOR_DURATION;
            TimerForTurningOffActuators.Elapsed += new ElapsedEventHandler(PowerDown);

            TimerForTurningOffHeat = new Timer();
            TimerForTurningOffHeat.Interval = ACTUATOR_DURATION;
            TimerForTurningOffHeat.Elapsed += new ElapsedEventHandler(PowerDownRecommendedActuators);


            Client.WriteLine("SetActiveCeilingAnimation|OFF");
            ToIdleState();
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
            PowerUp();
            StartActuatorTimer();
        }

        public void ActivateWhenHrIncreases(int current)
        {
            SetHrIsIncreasing(current);

            if (HrIsIncreasing)
            {
                Console.WriteLine("Act ON");
                PowerUp();
            }
            else
            {
                Console.WriteLine("Act OFF");
                PowerDown();
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
                FanIn();
            }
            else
            {
                FanOut();
            }

            HeatIn();
            TimerForTurningOffHeat.Start();
        }

        public void ResetActuators()
        {
            Console.WriteLine("Actuators reset.");
            PowerDown();
        }

        private void StartActuatorTimer()
        {
            TimerForTurningOffActuators.Start();
        }

        private void PowerUp()
        {
            HeatIn();
            FanIn();
            if (WithLight)
            {
                LightIn();
            }

        }
        private void PowerDown(object sender, ElapsedEventArgs e)
        {
            PowerDown();
            TimerForTurningOffActuators.Stop();
        }
        private void PowerDownRecommendedActuators(object sender, ElapsedEventArgs e)
        {
            HeatOut();
            LightOut();
            TimerForTurningOffHeat.Stop();
        }
        private void PowerDown()
        {
            Console.WriteLine("Actuators OFF");
            HeatOut();
            FanOut();
            LightOut();

        }

        public void TurnOff()
        {
            Client.WriteLine("SetHeaterIntensity|LEFT|" + NIL_POWER);
            Client.WriteLine("SetHeaterIntensity|RIGHT|" + NIL_POWER);
            Client.WriteLine("SetHeaterIntensity|SEAT_LEFT|" + NIL_POWER);
            Client.WriteLine("SetHeaterIntensity|SEAT_RIGHT|" + NIL_POWER);
            Client.WriteLine("SetHeaterIntensity|FRONT|" + NIL_POWER);
            Client.WriteLine("SetFanIntensity|FRONT_LEFT|" + NIL_POWER);
            Client.WriteLine("SetFanIntensity|FRONT_RIGHT|" + NIL_POWER);
            Client.WriteLine("SetFanIntensity|REAR_LEFT|" + NIL_POWER);
            Client.WriteLine("SetFanIntensity|REAR_RIGHT|" + NIL_POWER);
        }

        public void ToIdleState()
        {
            TimerForTurningOffActuators.Stop();
            TimerForTurningOffHeat.Stop();
            TurnOff();
            LightIn();
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
