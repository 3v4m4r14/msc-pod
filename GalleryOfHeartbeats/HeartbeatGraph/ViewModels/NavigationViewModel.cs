using GalleryOfHeartbeats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryOfHeartbeats.ViewModels

{
    public enum ViewTypes { ABOUT, GALLERY, RECORDING, SETTINGS, IDLE }
    class NavigationViewModel : ViewModelBase
    {
        private Settings Settings;

        private Dictionary<ViewTypes, ViewModelBase> ViewModels;

        private ViewModelBase selectedViewModel;

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

            ViewModels = new Dictionary<ViewTypes, ViewModelBase>()
            {
                [ViewTypes.ABOUT] = new AboutViewModel(),
                [ViewTypes.GALLERY] = new GalleryViewModel(Settings),
                [ViewTypes.RECORDING] = new RecordingViewModel(),
                [ViewTypes.SETTINGS] = new SettingsViewModel(Settings),
                [ViewTypes.IDLE] = new IdleViewModel()
            };

            SelectedViewModel = ViewModels[ViewTypes.ABOUT];
        }

        public void ChangeViewModel(ViewTypes _type)
        {
            SelectedViewModel = ViewModels[_type];
        }
    }
}
