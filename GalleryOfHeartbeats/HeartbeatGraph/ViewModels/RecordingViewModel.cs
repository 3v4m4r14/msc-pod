using GalleryOfHeartbeats.Model;
using GalleryOfHeartbeats.ViewModel.Commands;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace GalleryOfHeartbeats.ViewModels
{
    class RecordingViewModel : ViewModelBase
    {
        private const string PORT = "COM5"; //XRBase COM5
        private const float STARTING_TIME_IS_ZERO = 0.0f;
        private const int POLLING_INTERVAL = 500;
        private const int PLAYBACK_INTERVAL = 50;
        private const string GRAPH_TITLE = "";
        private const string FILENAME = "gallery.json";

        private float CurrentTime = 0.0f;
        private Timer GraphTimer;
        private Timer PlaybackTimer;

        private bool IsRecording = false;
        private bool GraphIsRunning = false;

        private readonly Connection Connection;
        private readonly FileHandler FileHandler;
        private HeartbeatTimer HeartbeatTimer;
        private AudioPlayer AudioPlayer;

        private Graph Graph;
        private Gallery Gallery;

        private GalleryItem CurrentRecordingItem = new GalleryItem();
        private List<int> CurrentRecordingData;

        private int PreviousHeartrate = 60;

        private bool isTimeForHeartbeat = false;
        public bool IsTimeForHeartbeat
        {
            get
            {
                return isTimeForHeartbeat;
            }
            set
            {
                isTimeForHeartbeat = value;
                OnPropertyChanged("IsTimeForHeartBeat");
            }
        }

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

        #region Recording
        public ICommand CommandStartRecording { get; private set; }
        public bool CanStartRecording(object param)
        {
            return !IsRecording && GraphIsRunning && !string.IsNullOrWhiteSpace(nameOfUser);
        }
        private void StartRecording(object param)
        {
            IsRecording = true;

            CreateNewRecording();

            Console.WriteLine("Recording: " + IsRecording);
        }

        private void CreateNewRecording()
        {
            CurrentRecordingItem = new GalleryItem();
            CurrentRecordingItem.Name = nameOfUser;
            CurrentRecordingItem.TimeOfRecording = DateTime.Now.ToString();
            CurrentRecordingItem.PollingRate = POLLING_INTERVAL;

            CurrentRecordingData = new List<int>();
        }

        public RelayCommand CommandStopRecording { get; private set; }
        public bool CanStopRecording(object param)
        {
            return IsRecording;
        }
        private void StopRecording(object param)
        {
            IsRecording = false;

            CurrentRecordingItem.Data = CurrentRecordingData;
            Gallery.GalleryItems.Add(CurrentRecordingItem);

            FileHandler.WriteToFile(Gallery);

            Console.WriteLine(CurrentRecordingItem.ToString());
            Console.WriteLine("Recording: " + IsRecording);

            ReloadGallery();

            NameOfUser = string.Empty;

            Console.WriteLine("Name of user is: " + NameOfUser);


            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(string.Format("Recording saved as '{0} {1}'", CurrentRecordingItem.Name, CurrentRecordingItem.TimeOfRecording));



        }

        #endregion

        #region Graph of HR
        public IList<DataPoint> AllPoints
        {
            get
            {
                return Graph.AllPoints;
            }
            set
            {
                Graph.AllPoints = value;
            }
        }
        public PlotModel GraphModel
        {
            get
            {
                return Graph.GraphModel;
            }
            set
            {
                Graph.GraphModel = value;
                OnPropertyChanged("GraphModel");
            }
        }
        #endregion

        public RecordingViewModel(string filename)
        {
            Connection = new Connection(PORT);
            AudioPlayer = new AudioPlayer();
            HeartbeatTimer = new HeartbeatTimer();

            PlaybackTimerInit();

            FileHandler = new FileHandler(filename);
            Gallery = FileHandler.GetGalleryFromFile();

            CurrentRecordingData = new List<int>();

            Graph = new Graph(GRAPH_TITLE);
            GraphTimerInit();

            CommandsInit();
        }

        public override void OnLoad()
        {
            UpdateGallery();
            CurrentRecordingData = new List<int>();
            Graph = new Graph(GRAPH_TITLE);
            CurrentTime = 0.0f;
            StartGraph();
        }

        public override void OffLoad()
        {
            if (IsRecording)
            {
                StopRecording(new object());
            }
            StopGraph();
        }

        private void PlaybackTimerInit()
        {
            PlaybackTimer = new Timer();
            PlaybackTimer.Interval = PLAYBACK_INTERVAL;
            PlaybackTimer.Elapsed += new ElapsedEventHandler(PlaybackTimerEvent);
        }

        private void PlaybackTimerEvent(object sender, ElapsedEventArgs e)
        {
            ProvideFeedback();
        }

        private void UpdateGallery()
        {
            Gallery = FileHandler.GetGalleryFromFile();
        }

        private void CommandsInit()
        {
            CommandStartRecording = new RelayCommand(StartRecording, CanStartRecording);
            CommandStopRecording = new RelayCommand(StopRecording, CanStopRecording);
        }

        private void ReloadGallery()
        {
            Gallery = FileHandler.GetGalleryFromFile();
            Console.WriteLine("Gallery reloaded");
        }

        private void StartGraph()
        {
            GraphIsRunning = true;
            GraphTimer.Start();
            PlaybackTimer.Start();
        }

        private void StopGraph()
        {
            GraphIsRunning = false;
            GraphTimer.Stop();
            PlaybackTimer.Stop();
        }

        #region Graph Timer Logic
        private void GraphTimerInit()
        {
            GraphTimer = new Timer();
            GraphTimer.Interval = POLLING_INTERVAL;
            GraphTimer.Elapsed += new ElapsedEventHandler(GraphTimerEvent);
        }

        private void GraphTimerEvent(object sender, EventArgs e)
        {
            GetDataFromSensor();

            AddHeartrateToGraph();
        }
        #endregion

        private void GetDataFromSensor()
        {
            string val = Connection.ReadFromPort();

            if (!string.IsNullOrEmpty(val))
            {
                ParseOutHeartrateFromConnectionData(val);
            }
            else
            {
                CurrentHeartrate = 0;
            }
        }

        private void AddHeartrateToGraph()
        {
            CurrentTime += (float)POLLING_INTERVAL / 1000;
            Graph.AddPoint(CurrentTime, CurrentHeartrate);
        }

        private void ParseOutHeartrateFromConnectionData(string val)
        {
            int ibiValue = 0;
            string firstval = "";

            for (int j = 0; j < val.Length; j++)
            {
                if (val[j] != '\n')
                {
                    firstval += val[j];
                }
                else
                {
                    break;
                }
            }

            int.TryParse(firstval, out ibiValue);

            if (ibiValue > 0)
            {
                CurrentHeartrate = (60000 / ibiValue); //http://www.psylab.com/html/default_heartrat.htm
            }

            CurrentRecordingData.Add(CurrentHeartrate);
        }

        private void ProvideFeedback()
        {
            
            if (HeartbeatTimer.TimeForHeartbeat(CurrentHeartrate))
            {
                IsTimeForHeartbeat = true;
                AudioPlayer.PlayHeartbeatAudio();
            }
            else
            {
                IsTimeForHeartbeat = false;
            }
            
        }
    }
}
