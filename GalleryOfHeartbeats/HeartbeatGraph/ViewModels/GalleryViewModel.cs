using GalleryOfHeartbeats.Model;
using GalleryOfHeartbeats.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace GalleryOfHeartbeats.ViewModels
{
    class GalleryViewModel : ViewModelBase
    {
        private const float STARTING_TIME_IS_ZERO = 0.0f;
        private const int POLLING_INTERVAL = 500;
        private const int POD_INTERVAL = 50;
        private const string FILENAME = "gallery.json";

        private readonly Connection Connection;
        private readonly FileHandler FileHandler;
        private HeartbeatTimer HeartbeatTimer;
        private Actuators Actuators;
        private AudioPlayer AudioPlayer;
        private Gallery Gallery;

        private float CurrentTime = 0.0f;
        private Timer PlaybackTimer;

        private bool IsPlayingBack = false;

        private int CurrentPlaybackPointer = 0;
        private int PreviousHeartrate = 60;

        #region Live Heartrate
        private int Heartrate;
        public int CurrentHeartrate
        {
            get
            {
                return Heartrate;
            }
            set
            {
                PreviousHeartrate = Heartrate;
                Heartrate = value;
                OnPropertyChanged("CurrentHeartrate");
            }
        }
        #endregion

        #region Playback
        public ObservableCollection<string> HeartbeatOptions
        {
            get
            {
                return Gallery.GetItemsAsStrings();
            }
        }
        public string SelectedItemName
        {
            get
            {
                return Gallery.SelectedItemName;
            }
            set
            {
                Gallery.SetSelectedItemById(value);
                OnPropertyChanged("SelectedItemName");
            }
        }
        public RelayCommand CommandStartPlayback { get; private set; }
        public bool CanStartPlayback(object param)
        {
            return !string.IsNullOrEmpty(Gallery.SelectedItemName);
        }
        private void StartPlayback(object param)
        {
            IsPlayingBack = true;


            CurrentTime = STARTING_TIME_IS_ZERO;
            CurrentPlaybackPointer = 0;
            PlaybackTimer.Start();

        }

        public RelayCommand CommandStopPlayback { get; private set; }
        public bool CanStopPlayback(object param)
        {
            return IsPlayingBack;
        }
        private void StopPlayback(object param)
        {
            PlaybackTimer.Stop();
            IsPlayingBack = false;
        }
        #endregion

        public GalleryViewModel()
        {
            Connection = new Connection();
            Actuators = new Actuators();
            AudioPlayer = new AudioPlayer();
            HeartbeatTimer = new HeartbeatTimer();

            PlaybackTimerInit();

            FileHandler = new FileHandler(FILENAME);
            Gallery = FileHandler.GetGalleryFromFile();

            CommandsInit();
        }

        private void CommandsInit()
        {
            CommandStartPlayback = new RelayCommand(StartPlayback, CanStartPlayback);
            CommandStopPlayback = new RelayCommand(StopPlayback, CanStopPlayback);
        }

        #region Playback Timer Logic
        private void PlaybackTimerInit()
        {
            PlaybackTimer = new Timer();
            PlaybackTimer.Interval = POD_INTERVAL;
            PlaybackTimer.Elapsed += new ElapsedEventHandler(PlaybackTimerEvent);
        }

        private void PlaybackTimerEvent(object sender, ElapsedEventArgs e)
        {
            ProvideFeedback();
        }

        private void ProvideFeedback()
        {
            if (HeartbeatTimer.TimeForHeartbeat(CurrentHeartrate))
            {
                Actuators.ActivateWhenHrIncreases(CurrentHeartrate);
                AudioPlayer.PlayHeartbeatAudio();
            }
        }
        #endregion
    }
}
