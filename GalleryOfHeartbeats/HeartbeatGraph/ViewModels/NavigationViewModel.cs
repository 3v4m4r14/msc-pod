using GalleryOfHeartbeats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace GalleryOfHeartbeats.ViewModels

{
    public enum ViewTypes { ABOUT, GALLERY, RECORDING, SETTINGS, IDLE }
    class NavigationViewModel : ViewModelBase
    {
        private const string FILENAME = "gallery.json";
        private const int IDLE_INTERVAL = 50000;

        private Settings Settings;

        private Dictionary<ViewTypes, ViewModelBase> ViewModels;

        private ViewModelBase selectedViewModel;
        private ViewModelBase previousViewModel;

        private Timer IdleTimer;

        public ViewModelBase SelectedViewModel
        {
            get
            {
                return selectedViewModel;
            }
            set
            {
                selectedViewModel = value;
                OnPropertyChanged("SelectedViewModel");
            }
        }

        public NavigationViewModel()
        {
            Settings = new Settings();

            IdleTimer = new Timer();
            IdleTimer.Interval = IDLE_INTERVAL;
            IdleTimer.Elapsed += new ElapsedEventHandler(IdleTimerEvent);
            IdleTimer.Start();

            ViewModels = new Dictionary<ViewTypes, ViewModelBase>()
            {
                [ViewTypes.ABOUT] = new AboutViewModel(),
                [ViewTypes.GALLERY] = new GalleryViewModel(Settings, FILENAME),
                [ViewTypes.RECORDING] = new RecordingViewModel(FILENAME),
                [ViewTypes.SETTINGS] = new SettingsViewModel(Settings),
                [ViewTypes.IDLE] = new IdleViewModel()
            };

            SelectedViewModel = ViewModels[ViewTypes.IDLE];
        }

        public void ChangeViewModel(ViewTypes _type)
        {
            
            SelectedViewModel.OffLoad();
            SelectedViewModel = ViewModels[_type];
            SelectedViewModel.OnLoad();

            IdleTimer.Stop();
            IdleTimer.Start();
        }

        private void IdleTimerEvent(object sender, EventArgs e)
        {
            previousViewModel = SelectedViewModel;
            ChangeViewModel(ViewTypes.IDLE);
        }

    }
}
