using GalleryOfHeartbeats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryOfHeartbeats.ViewModels
{
    class SettingsViewModel : ViewModelBase
    {
        private Settings Settings;


        public PlaybackMode PlaybackMode
        {
            get
            {
                return Settings.Mode;
            }
            set
            {
                if (value == PlaybackMode.RECOMMENDED)
                {
                    Settings.GetRecommendedSettings();
                    OnPropertyChanged("Heat1");
                    OnPropertyChanged("Heat2");
                    OnPropertyChanged("Fan1");
                    OnPropertyChanged("Fan2");
                    OnPropertyChanged("LightStatus");
                }
                Settings.Mode = value;
                OnPropertyChanged("PlaybackMode");
            }
        }

        public float Heat1
        {
            get
            {
                return Settings.MinHeatIntensity;
            }
            set
            {
                Settings.MinHeatIntensity = value;
                OnPropertyChanged("Heat1");
            }
        }

        public float Heat2
        {
            get
            {
                return Settings.MaxHeatIntensity;
            }
            set
            {
                Settings.MaxHeatIntensity = value;
                OnPropertyChanged("Heat2");
            }
        }

        public float Fan1
        {
            get
            {
                return Settings.MinFanIntensity;
            }
            set
            {
                Settings.MinFanIntensity = value;
                OnPropertyChanged("Fan1");
            }
        }

        public float Fan2
        {
            get
            {
                return Settings.MaxFanIntensity;
            }
            set
            {
                Settings.MaxFanIntensity = value;
                OnPropertyChanged("Fan2");
            }
        }

        public bool LightStatus
        {
            get
            {
                return Settings.LightOn;
            }
            set
            {
                Settings.LightOn = value;
                OnPropertyChanged("LightStatus");
            }
        }


        public SettingsViewModel(Settings settings)
        {
            this.Settings = settings;

            Console.WriteLine("-------SettingsViewModel-------" + Settings.ToString());
        }
    }
}
