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

        public SettingsViewModel(Settings settings)
        {
            this.Settings = settings;

            Console.WriteLine("-------SettingsViewModel-------" + Settings.ToString());
        }
    }
}
