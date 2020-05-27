using GalleryOfHeartbeats.Model;
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
    class RecordingViewModel
    {
        private const string PORT = "COM5";
        private const float STARTING_TIME_IS_ZERO = 0.0f;
        private const int POLLING_INTERVAL = 500;
        private const string GRAPH_TITLE = "Heart rate (bpm)";
        private const string FILENAME = "gallery.json";

        private float CurrentTime = 0.0f;
        private Timer GraphTimer;

        private bool IsRecording = false;
        private bool GraphIsRunning = false;

        private readonly Connection Connection;
        private readonly FileHandler FileHandler;
        private HeartbeatTimer HeartbeatTimer;
        private AudioPlayer AudioPlayer;

        private Graph Graph;

        private GalleryItem CurrentRecordingItem;
        private List<int> CurrentRecordingData;

        private int PreviousHeartrate = 60;

        #region Username (ID) for the Recording
        private string nameOfUser = "";
        public string NameOfUser
        {
            get
            {
                return nameOfUser;
            }
            set
            {
                nameOfUser = value;
                OnPropertyChanged("NameOfUser");
            }
        }
        #endregion

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


        public RecordingViewModel()
        {
            Connection = new Connection(true);

        }


        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
