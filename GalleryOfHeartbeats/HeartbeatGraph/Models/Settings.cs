using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryOfHeartbeats.Models
{
    public enum PlaybackMode { PER_BEAT, PER_INCREASE }

    public class Settings
    {

        public PlaybackMode Mode;
        public float MinHeatIntensity { get; set; }
        public float MaxHeatIntensity { get; set; }
        public float MinFanIntensity { get; set; }
        public float MaxFanIntensity { get; set; }
        public bool LightOn { get; set; }

        public Settings(PlaybackMode mode, float minHeat, float maxHeat, float minFan, float maxFan, bool lightOn)
        {
            this.Mode = mode;
            this.MinHeatIntensity = minHeat;
            this.MaxHeatIntensity = maxHeat;
            this.MinFanIntensity = minFan;
            this.MaxFanIntensity = maxFan;
            this.LightOn = lightOn;
        }

        override public string ToString()
        {
            return string.Format("Settings\nMode: {0}\nHeat: {1}...{2}\nFan: {3}...{4}\nLightOn: {5}", Mode, MinHeatIntensity, MaxHeatIntensity, MinFanIntensity, MaxFanIntensity, LightOn);
        }
    }
}
