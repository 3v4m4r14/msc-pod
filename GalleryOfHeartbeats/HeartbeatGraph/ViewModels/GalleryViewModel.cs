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
        private Timer GraphTimer;
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
                Console.WriteLine("Selecting: " + value);
                if (IsPlayingBack && Gallery.SelectedItemIsTheSameAs(value))
                {
                    PausePlayback();
                }
                else if (!IsPlayingBack && Gallery.SelectedItemIsTheSameAs(value))
                {
                    ContinuePlayback();
                }
                else {
                    Gallery.SetSelectedItemById(value);
                    StartPlayback();
                    OnPropertyChanged("SelectedItemName");
                }
            }
        }

        private void ContinuePlayback()
        {
            IsPlayingBack = true;
            GraphTimer.Start();
            PlaybackTimer.Start();
            Console.WriteLine("Continue");
        }

        private void PausePlayback()
        {
            IsPlayingBack = false;
            GraphTimer.Stop();
            PlaybackTimer.Stop();
            Console.WriteLine("Paused");
        }

        public bool CanStartPlayback()
        {
            return !string.IsNullOrEmpty(Gallery.SelectedItemName);
        }


        private void StartPlayback()
        {
            if (CanStartPlayback()) {
                IsPlayingBack = true;

                Console.WriteLine("Playing back: " + IsPlayingBack);

                CurrentPlaybackPointer = 0;
                PlaybackTimer.Start();

                CurrentTime = STARTING_TIME_IS_ZERO;
                GraphTimer.Interval = Gallery.SelectedItem.PollingRate;
                RestartGraphTimer();
            }    
        }

        private void StopPlayback()
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


            GraphTimerInit();
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

        #region Graph Timer Logic
        private void GraphTimerInit()
        {
            GraphTimer = new Timer();
            GraphTimer.Interval = POLLING_INTERVAL;
            GraphTimer.Elapsed += new ElapsedEventHandler(GraphTimerEvent);
        }

        private void RestartGraphTimer()
        {
            GraphTimer.Stop();
            GraphTimer.Start();
        }

        private void GraphTimerEvent(object sender, EventArgs e)
        {
            if (IsPlayingBack)
            {
                if (Gallery.HasNoMoreData(CurrentPlaybackPointer))
                {
                    StopPlayback();
                }
                else
                {
                    GetDataFromGallery();
                }
            }
            //ProvideFeedback();
        }
        #endregion

        private void GetDataFromGallery()
        {
            CurrentHeartrate = Gallery.GetSelectedItemDataValAt(CurrentPlaybackPointer);
            CurrentPlaybackPointer++;
        }
    }
}
