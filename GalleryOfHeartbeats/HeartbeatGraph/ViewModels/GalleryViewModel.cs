using GalleryOfHeartbeats.Model;
using GalleryOfHeartbeats.Models;
using GalleryOfHeartbeats.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;

namespace GalleryOfHeartbeats.ViewModels
{
    class GalleryViewModel : ViewModelBase
    {
        private Settings Settings;

        private const float STARTING_TIME_IS_ZERO = 0.0f;
        private const int POLLING_INTERVAL = 500;
        private const int POD_INTERVAL = 50;

        private readonly FileHandler FileHandler;
        private HeartbeatTimer HeartbeatTimer;
        private Actuators Actuators;
        private AudioPlayer AudioPlayer;
        private Gallery Gallery;

        private float CurrentTime = 0.0f;
        private Timer GraphTimer;
        private Timer PlaybackTimer;

        private bool isPlayingBack = false;
        public bool IsPlayingBack
        {
            get
            {
                return isPlayingBack;
            }
            set
            {
                isPlayingBack = value;
                OnPropertyChanged("IsPlayingBack");
            }
        }

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

        #region List items
        public ObservableCollection<GalleryItem> HeartbeatOptions
        {
            get
            {
                return Gallery.GalleryItems;
            }
        }

        #endregion

        private ICommand _command;
        public ICommand ClickOnListItemCommand
        {
            get
            {
                return _command ?? (_command = new RelayCommand(x =>
                {
                    DoStuff(x as GalleryItem);
                }));
            }
        }

        public void DoStuff(GalleryItem item)
        {
            Console.WriteLine("Item: " + item.Name + Gallery.SelectedItem.Name);

            if (item.Equals(Gallery.SelectedItem) && IsPlayingBack)
            {
                Console.WriteLine("Pausing");
                PausePlayback();
            }
            else if (item.Equals(Gallery.SelectedItem) && !IsPlayingBack)
            {
                Console.WriteLine("Continuing");
                ContinuePlayback();
            }
            else
            {
                Gallery.SelectedItem = item;
                StartPlayback();
            }
            Gallery.SelectedItem = item;
        }

        #region Playback
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
            Actuators.ToIdleState();
        }

        public bool CanStartPlayback()
        {
            return Gallery.SelectedItem != null;
        }
        private void StartPlayback()
        {
            StopPlayback();

            if (CanStartPlayback()) {
                IsPlayingBack = true;

                Console.WriteLine("Playing back: " + IsPlayingBack);

                PlaybackTimer.Start();

                GraphTimer.Interval = Gallery.SelectedItem.PollingRate;
                RestartGraphTimer();
            }    
        }

        private void StopPlayback()
        {
            PlaybackTimer.Stop();
            IsPlayingBack = false;
            CurrentPlaybackPointer = 0;
            CurrentTime = STARTING_TIME_IS_ZERO;
            Actuators.ToIdleState();
        }
        #endregion


        public GalleryViewModel(Settings settings, string filename)
        {
            this.Settings = settings;

            Actuators = new Actuators(settings);
            AudioPlayer = new AudioPlayer();
            HeartbeatTimer = new HeartbeatTimer();

            PlaybackTimerInit();

            FileHandler = new FileHandler(filename);
            UpdateGallery();


            GraphTimerInit();

        }

        override public void OnLoad()
        {
            UpdateGallery();
            Actuators.UpdateSettings(Settings);
            Console.WriteLine("-----GalleryViewModel-----" + Settings.ToString());
        }

        override public void OffLoad()
        {
            StopPlayback();
            GraphTimer.Stop();
            PlaybackTimer.Stop();
            Console.WriteLine("Gallery offloaded");
        }

        private void UpdateGallery()
        {
            Gallery = FileHandler.GetGalleryFromFile();
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
                if (Settings.Mode.Equals(PlaybackMode.PER_BEAT))
                {
                    Actuators.ActivateWithHeartbeat();
                }
                else if (Settings.Mode.Equals(PlaybackMode.PER_INCREASE))
                {
                    Actuators.ActivateWhenHrIncreases(CurrentHeartrate);
                }
                else if (Settings.Mode.Equals(PlaybackMode.RECOMMENDED))
                {
                    Actuators.RecommendedExperience(CurrentHeartrate);
                }
                
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
