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

namespace GalleryOfHeartbeats.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private const float STARTING_TIME_IS_ZERO = 0.0f;
        private const int POLLING_INTERVAL = 1000;

        private static readonly object MOCK_PARAM = new object();

        private float CurrentTime = 0.0f;
        private Timer GraphTimer;

        private bool IsRecording = false;
        private bool IsPlayingBack = false;
        private bool GraphIsRunning = false;

        private readonly Connection Connection;
        private readonly FileHandler FileHandler;

        private Graph Graph;
        private Gallery Gallery;

        private GalleryItem CurrentRecordingItem;
        private List<int> CurrentRecordingData;

        private int CurrentPlaybackPointer = 0;


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
                ChangeProperty("NameOfUser");
            }
        }
        #endregion

        #region Live Heartrate
        private int Heartrate;
        public string CurrentHeartrate
        {
            get
            {
                return "Heart rate: " + Heartrate;
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
                ChangeProperty("ConnectionOptions");
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
                ChangeProperty("SelectedPort");
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
                ChangeProperty("GraphModel");
            }
        }

        public RelayCommand CommandShowGraph { get; private set; }
        public bool CanShowGraph(object param)
        {
            if (!GraphIsRunning) return Connection.PortIsReady();
            return false;
        }
        private void ShowGraph(object param)
        {
            RefreshGallery();
            ClearGraph(MOCK_PARAM);
            GraphIsRunning = true;
            RestartGraphTimer();
            MessageBox.Show("Clip the sensor on your ear or finger.\n\nWait for the graph to stabilise before you start recording.\nA stable graph looks like seawaves.");
        }

        public RelayCommand CommandPauseGraph { get; private set; }
        public bool CanPauseGraph(object param)
        {
            return GraphIsRunning && !IsRecording && !IsPlayingBack;
        }
        public void PauseGraph(object param)
        {
            if (IsRecording) { StopRecording(MOCK_PARAM); }

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
                Console.WriteLine("Selected item name is: " + Gallery.SelectedItemName);
                return Gallery.SelectedItemName;
            }
            set
            {
                Gallery.SetSelectedItemById(value);
                ChangeProperty("SelectedItemName");
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
            Console.WriteLine("Playback started: " + Gallery.SelectedItemName);

            IsPlayingBack = true;
            GraphIsRunning = true;

            CurrentTime = STARTING_TIME_IS_ZERO;
            CurrentPlaybackPointer = 0;
            GraphTimer.Interval = Gallery.SelectedItem.PollingRate;

            RestartGraphTimer();
        }

        public RelayCommand CommandStopPlayback { get; private set; }
        public bool CanStopPlayback(object param)
        {
            return GraphIsRunning;
        }
        private void StopPlayback(object param)
        {
            ClearGraph(MOCK_PARAM);
        }
        #endregion

        private void RefreshGallery()
        {
            Gallery = FileHandler.GetGalleryFromFile();
            SelectedItemName = "";
            ChangeProperty("HeartbeatOptions");
            ChangeProperty("SelectedItemName");
        }


        public MainViewModel()
        {
            Connection = new Connection();

            Graph = new Graph("Heart rate (bpm)");
            GraphTimerInit();

            FileHandler = new FileHandler("gallery.json");
            Gallery = FileHandler.GetGalleryFromFile();

            CurrentRecordingData = new List<int>();

            //PopulateGalleryWithMockData();

            CommandsInit();

        }

        private void CommandsInit()
        {
            CommandStartRecording = new RelayCommand(StartRecording, CanStartRecording);
            CommandStopRecording = new RelayCommand(StopRecording, CanStopRecording);
            CommandShowGraph = new RelayCommand(ShowGraph, CanShowGraph);
            CommandPauseGraph = new RelayCommand(PauseGraph, CanPauseGraph);
            CommandClearGraph = new RelayCommand(ClearGraph, CanClearGraph);
            CommandStartPlayback = new RelayCommand(StartPlayback, CanStartPlayback);
            CommandStopPlayback = new RelayCommand(StopPlayback, CanStopPlayback);
        }

        private void PopulateGalleryWithMockData()
        {
            GalleryItem item0 = new GalleryItem()
            {
                Name = "Eva",
                TimeOfRecording = DateTime.Now.ToString(),
                PollingRate = POLLING_INTERVAL,
                Data = new List<int>() {
                    60, 64, 68, 76, 90, 84, 71, 59
                }
            };

            GalleryItem item1 = new GalleryItem()
            {
                Name = "Maria",
                TimeOfRecording = DateTime.Now.ToString(),
                PollingRate = POLLING_INTERVAL,
                Data = new List<int>() {
                    10, 23, 24, 29, 34, 35, 45, 50
                }
            };

            Gallery.GalleryItems.Add(item0);
            Gallery.GalleryItems.Add(item1);
        }

        private void GraphTimerInit()
        {
            GraphTimer = new Timer();
            GraphTimer.Elapsed += new ElapsedEventHandler(TimerEvent);
            GraphTimer.Interval = POLLING_INTERVAL;
        }

        //start the timer for polling
        private void RestartGraphTimer()
        {
            GraphTimer.Stop();
            GraphTimer.Start();
        }

        //event that runs every milisecondinterval
        private void TimerEvent(object sender, EventArgs e)
        {
            if (IsPlayingBack)
            {
                Console.WriteLine("Playing back");
                GetDataFromGallery();
            }
            else
            {
                GetDataFromSensor();
            }

            

            AddHeartrateToGraph();
        }

        private void GetDataFromGallery()
        {
            Heartrate = Gallery.GetSelectedItemDataValAt(CurrentPlaybackPointer);
            Console.WriteLine("Data from gallery is: " + Heartrate);
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
                Heartrate = 0;
            }
        }

        private void AddHeartrateToGraph()
        {
            //get x point
            CurrentTime += (float)POLLING_INTERVAL / 1000;

            //add points to graph
            Graph.AddPoint(CurrentTime, Heartrate);
        }

        private void ParseOutHeartrateFromConnectionData(string val)
        {
            //get first value
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

            Console.WriteLine(ibiValue);

            //convert ibi value to heartrate
            if (ibiValue > 0)
            {
                Heartrate = (60000 / ibiValue); //http://www.psylab.com/html/default_heartrat.htm
                ChangeProperty("CurrentHeartbeat");
            }

            CurrentRecordingData.Add(Heartrate);
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void ChangeProperty(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}