using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryOfHeartbeats.Models
{
    public enum PlaybackMode { PER_BEAT, PER_INCREASE, RECOMMENDED }

    public class Settings
    {
        private const float REC_MIN_HEAT = 0.1f;
        private const float REC_MAX_HEAT = 0.4f;
        private const float REC_MIN_AIR = 0.1f;
        private const float REC_MAX_AIR = 0.3f;
        private const bool REC_LIGHT = false;

        public PlaybackMode Mode = PlaybackMode.RECOMMENDED;
        public float MinHeatIntensity { get; set; } = REC_MIN_HEAT;
        public float MaxHeatIntensity { get; set; } = REC_MAX_HEAT;
        public float MinFanIntensity { get; set; } = REC_MIN_AIR;
        public float MaxFanIntensity { get; set; } = REC_MAX_AIR;
        public bool LightOn { get; set; } = REC_LIGHT;

        public Settings(PlaybackMode mode, float minHeat, float maxHeat, float minFan, float maxFan, bool lightOn)
        {
            this.Mode = mode;
            this.MinHeatIntensity = minHeat;
            this.MaxHeatIntensity = maxHeat;
            this.MinFanIntensity = minFan;
            this.MaxFanIntensity = maxFan;
            this.LightOn = lightOn;
        }

        public void GetRecommendedSettings()
        {
            this.Mode = PlaybackMode.RECOMMENDED;
            this.MinHeatIntensity = REC_MIN_HEAT;
            this.MaxHeatIntensity = REC_MAX_HEAT;
            this.MinFanIntensity = REC_MIN_AIR;
            this.MaxFanIntensity = REC_MAX_AIR;
            this.LightOn = REC_LIGHT;
        }

        public Settings() { }

        override public string ToString()
        {
            return string.Format("Settings\nMode: {0}\nHeat: {1}...{2}\nFan: {3}...{4}\nLightOn: {5}", Mode, MinHeatIntensity, MaxHeatIntensity, MinFanIntensity, MaxFanIntensity, LightOn);
        }
    }
}
