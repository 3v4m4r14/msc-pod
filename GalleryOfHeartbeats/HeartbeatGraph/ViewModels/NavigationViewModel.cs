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
        private const string FILENAME = "gallery.json";

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
                [ViewTypes.GALLERY] = new GalleryViewModel(Settings, FILENAME),
                [ViewTypes.RECORDING] = new RecordingViewModel(FILENAME),
                [ViewTypes.SETTINGS] = new SettingsViewModel(Settings),
                [ViewTypes.IDLE] = new IdleViewModel()
            };

            SelectedViewModel = ViewModels[ViewTypes.ABOUT];
        }

        public void ChangeViewModel(ViewTypes _type)
        {
            SelectedViewModel.OffLoad();
            SelectedViewModel = ViewModels[_type];
            SelectedViewModel.OnLoad();
        }

    }
}
