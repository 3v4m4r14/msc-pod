// Gallery of Heartbeats
// Author: Eva Maria Veitmaa
// Date: 2020


using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management;
using System.Timers;
using System.IO.Ports;
using System.Text;
using System.Windows.Input;
using System.Security.Cryptography.X509Certificates;
using GalleryOfHeartbeats.Model;
using GalleryOfHeartbeats.ViewModel.Commands;
using System.ComponentModel;
using System.Windows;
using System.IO;
using System.Windows.Threading;

namespace GalleryOfHeartbeats.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private const float STARTING_TIME_IS_ZERO = 0.0f;
        private const int POLLING_INTERVAL = 500;
        private const int POD_INTERVAL = 50;
        private const string GRAPH_TITLE = "Heart rate (bpm)";
        private const string FILENAME = "gallery.json";

        private static readonly object MOCK_PARAM = new object();

        private float CurrentTime = 0.0f;
        private Timer GraphTimer;
        private Timer PlaybackTimer;

        private bool IsRecording = false;
        private bool IsPlayingBack = false;
        private bool GraphIsRunning = false;

        private readonly Connection Connection;
        private readonly FileHandler FileHandler;
        private HeartbeatTimer HeartbeatTimer;
        private Actuators Actuators;
        private AudioPlayer AudioPlayer;

        private Graph Graph;
        private Gallery Gallery;

        private GalleryItem CurrentRecordingItem;
        private List<int> CurrentRecordingData;

        private int CurrentPlaybackPointer = 0;
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

        #region Port Connection for HR Sensor
        public ObservableCollection<string> ConnectionOptions
        {
            get
            {
                return Connection.Options;
            }
            set
            {
                Connection.Options = value;
                OnPropertyChanged("ConnectionOptions");
            }
        }

        public string SelectedPort
        {
            get
            {
                return Connection.SelectedPort;
            }
            set
            {
                Connection.SelectedPort = value;
                OnPropertyChanged("SelectedPort");
            }
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

        public RelayCommand CommandShowGraph { get; private set; }
        public bool CanShowGraph(object param)
        {
            if (!GraphIsRunning && !IsRecording) return Connection.PortIsReady();
            return false;
        }
        private void ShowGraph(object param)
        {
            ClearGraph(MOCK_PARAM);
            RefreshGallery();

            GraphIsRunning = true;
            RestartGraphTimer();
            PlaybackTimer.Start();

            MessageBox.Show("Clip the sensor on your ear or finger.\n\nWait for the graph to stabilise before you start recording.\nA stable graph looks like seawaves.");
        }

        public RelayCommand CommandStopGraph { get; private set; }
        public bool CanStopGraph(object param)
        {
            return GraphIsRunning && !IsRecording && !IsPlayingBack;
        }
        public void StopGraph(object param)
        {
            if (IsRecording) { StopRecording(MOCK_PARAM); }

            PlaybackTimer.Stop();

            StopPlotting();
            Console.WriteLine("Graph paused");
        }

        public RelayCommand CommandClearGraph { get; private set; }
        public bool CanClearGraph(object param)
        {
            return !IsRecording;
        }
        public void ClearGraph(object param)
        {
            if (IsRecording) { StopRecording(MOCK_PARAM); }

            StopPlotting();

            CurrentTime = STARTING_TIME_IS_ZERO;

            Graph.ResetGraph();

            Console.WriteLine("Graph cleared");
        }

        private void StopPlotting()
        {
            GraphTimer.Stop();
            GraphIsRunning = false;
            IsPlayingBack = false;
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

            StopPlotting();

            CurrentRecordingItem.Data = CurrentRecordingData;
            Gallery.GalleryItems.Add(CurrentRecordingItem);

            FileHandler.WriteToFile(Gallery);

            Console.WriteLine(CurrentRecordingItem.ToString());
            Console.WriteLine("Recording: " + IsRecording);

            RefreshGallery();

            NameOfUser = string.Empty;

            MessageBox.Show(String.Format("Recording saved as '{0} {1}'", CurrentRecordingItem.Name, CurrentRecordingItem.TimeOfRecording));
            

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
            return !IsRecording && !GraphIsRunning && !string.IsNullOrEmpty(Gallery.SelectedItemName);
        }
        private void StartPlayback(object param)
        {
            ClearGraph(MOCK_PARAM);

            IsPlayingBack = true;
            GraphIsRunning = true;

            CurrentTime = STARTING_TIME_IS_ZERO;
            CurrentPlaybackPointer = 0;
            GraphTimer.Interval = Gallery.SelectedItem.PollingRate;

            RestartGraphTimer();
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

            GraphTimer.Interval = POLLING_INTERVAL;
            ClearGraph(MOCK_PARAM);
        }
        #endregion

        public MainViewModel()
        {
            Connection = new Connection();
            Actuators = new Actuators();
            AudioPlayer = new AudioPlayer();
            HeartbeatTimer = new HeartbeatTimer();

            Graph = new Graph(GRAPH_TITLE);
            GraphTimerInit();

            PlaybackTimerInit();

            FileHandler = new FileHandler(FILENAME);
            Gallery = FileHandler.GetGalleryFromFile();

            CurrentRecordingData = new List<int>();

            CommandsInit();
        }

        private void CommandsInit()
        {
            CommandStartRecording = new RelayCommand(StartRecording, CanStartRecording);
            CommandStopRecording = new RelayCommand(StopRecording, CanStopRecording);
            CommandShowGraph = new RelayCommand(ShowGraph, CanShowGraph);
            CommandStopGraph = new RelayCommand(StopGraph, CanStopGraph);
            CommandClearGraph = new RelayCommand(ClearGraph, CanClearGraph);
            CommandStartPlayback = new RelayCommand(StartPlayback, CanStartPlayback);
            CommandStopPlayback = new RelayCommand(StopPlayback, CanStopPlayback);
        }

        private void RefreshGallery()
        {
            Gallery = FileHandler.GetGalleryFromFile();
            SelectedItemName = "";
            OnPropertyChanged("HeartbeatOptions");
            OnPropertyChanged("SelectedItemName");
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

            //Actuators.OnHeartrateChangeBasic(PreviousHeartrate, CurrentHeartrate);

            if (HeartbeatTimer.TimeForHeartbeat(CurrentHeartrate))
            {
                Actuators.OnHeartrateChangeAdvanced();
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
                    StopPlayback(MOCK_PARAM);
                }
                else {
                    GetDataFromGallery();
                }
            }
            else
            {
                GetDataFromSensor();
                if (HeartbeatTimer.TimeForHeartbeat(CurrentHeartrate))
                {
                    //Actuators.OnHeartrateChangeAdvanced();
                    AudioPlayer.PlayHeartbeatAudio();
                }
            }

            AddHeartrateToGraph();
        }
        #endregion

        private void GetDataFromGallery()
        {
            CurrentHeartrate = Gallery.GetSelectedItemDataValAt(CurrentPlaybackPointer);
            CurrentPlaybackPointer++;
        }

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

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}