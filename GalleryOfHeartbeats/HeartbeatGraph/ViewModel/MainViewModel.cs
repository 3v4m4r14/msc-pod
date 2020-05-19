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

namespace GalleryOfHeartbeats.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private static readonly float DEFAULT_TIME = 0.0f;

        public int Heartrate;

        private float CurrentTime = 0.0f;
        private int PollingInterval = 1000;
        private Timer GraphTimer;

        private bool IsRecording = false;
        private bool GraphIsRunning = false;


        private FileHandler FileHandler;
        private Gallery Gallery;

        private GalleryItem CurrentRecordingItem;
        private List<int> CurrentRecordingData;

        public string NameOfUser { get; set; }


        public string CurrentHeartbeat
        {
            get
            {
                return "heartrate: " + Heartrate;
            }
        }

        #region Port Connection for HR
        public Connection Connection { get; }

        //list of the available com ports
        public ObservableCollection<string> Options
        {
            get
            {
                return Connection.Options;
            }
            set
            {
                Connection.Options = value;
            }
        }

        //selected com port
        public string SelectedPort
        {
            get
            {
                return Connection.SelectedPort;
            }
            set
            {
                Connection.SelectedPort = value;
            }
        }
        #endregion

        #region Graph of HR
        public Graph Graph { get; set; }

        //all datapoints 
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

        //model for the graph
        public PlotModel GraphModel
        {
            get
            {
                return Graph.GraphModel;
            }
            set
            {
                Graph.GraphModel = value;
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
            GraphIsRunning = true;
            RestartGraphTimer();
        }

        public RelayCommand CommandPauseGraph { get; private set; }
        public bool CanPauseGraph(object param)
        {
            return GraphIsRunning;
        }
        public void PauseGraph(object param)
        {
            if (IsRecording) { StopRecording(new object()); }

            StopPlotting();
            Console.WriteLine("Graph paused");
        }

        public RelayCommand CommandClearGraph { get; private set; }
        public bool CanClearGraph(object param)
        {
            return true;
        }
        public void ClearGraph(object param)
        {
            if (IsRecording) { StopRecording(new object()); }

            StopPlotting();

            CurrentTime = DEFAULT_TIME;
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
            return !IsRecording && GraphIsRunning && !string.IsNullOrWhiteSpace(NameOfUser);
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
            CurrentRecordingItem.Name = NameOfUser;
            CurrentRecordingItem.TimeOfRecording = DateTime.Now.ToString();
            CurrentRecordingItem.PollingRate = PollingInterval;

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
        }
        #endregion

        

        

        public MainViewModel()
        {
            Connection = new Connection();

            Graph = new Graph("Heart rate (bpm)");
            GraphTimerInit();

            FileHandler = new FileHandler("gallery.json");
            Gallery = FileHandler.GetGalleryFromFile();

            CurrentRecordingData = new List<int>();

            //PopulateGalleryWithMockData();

            CommandStartRecording = new RelayCommand(StartRecording, CanStartRecording);
            CommandStopRecording = new RelayCommand(StopRecording, CanStopRecording);
            CommandShowGraph = new RelayCommand(ShowGraph, CanShowGraph);
            CommandPauseGraph = new RelayCommand(PauseGraph, CanPauseGraph);
            CommandClearGraph = new RelayCommand(ClearGraph, CanClearGraph);

        }

        private void PopulateGalleryWithMockData()
        {
            GalleryItem item0 = new GalleryItem()
            {
                Name = "Eva",
                TimeOfRecording = DateTime.Now.ToString(),
                PollingRate = PollingInterval,
                Data = new List<int>() {
                    60, 64, 68, 76, 90, 84, 71, 59
                }
            };

            GalleryItem item1 = new GalleryItem()
            {
                Name = "Maria",
                TimeOfRecording = DateTime.Now.ToString(),
                PollingRate = PollingInterval,
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
            GraphTimer.Interval = PollingInterval;
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
            string val = Connection.ReadFromPort();

            if (!string.IsNullOrEmpty(val))
            {
                ParseOutHeartrateFromConnectionData(val);
            }
            else
            {
                Heartrate = 0;
            }

            //get x point
            CurrentTime += (float)PollingInterval / 1000;

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

            CurrentRecordingData.Add(ibiValue);
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