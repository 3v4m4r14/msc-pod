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
        public int Heartrate;

        private float CurrentTime = 0.0f;
        private int PollingInterval = 1000;
        private Timer GraphTimer;

        private bool IsRecording = false;
        private bool IsShowingGraph = false;


        private FileHandler FileHandler;
        private Gallery Gallery;

        

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
        public Graph Graph { get; }

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
        #endregion


        public ICommand CommandStartRecording { get; private set; }
        public bool CanStartRecording(object param)
        {
            return !IsRecording && !string.IsNullOrWhiteSpace(NameOfUser);
        }
        private void StartRecording(object param)
        {
            IsRecording = true;
            FileHandler.WriteToFile(Gallery);
            Console.WriteLine("Recording: " + IsRecording);
        }

        public RelayCommand CommandStopRecording { get; private set; }
        public bool CanStopRecording(object param)
        {
            return IsRecording;
        }
        private void StopRecording(object param)
        {
            IsRecording = false;
            Console.WriteLine(FileHandler.ReadFromFile());
            Console.WriteLine("Recording: " + IsRecording);
        }

        public RelayCommand CommandShowGraph { get; private set; }
        public bool CanShowGraph(object param)
        {
            if (!IsShowingGraph) return Connection.PortIsReady();
            return false;
        }
        private void ShowGraph(object param)
        {
            IsShowingGraph = true;
            ResetGraphTimer();
        }

        public MainViewModel()
        {
            Connection = new Connection();

            Graph = new Graph("Heartrate");
            GraphTimer = new Timer();
            GraphTimer.Elapsed += new ElapsedEventHandler(TimerEvent);
            GraphTimer.Interval = PollingInterval;

            FileHandler = new FileHandler("gallery.json");
            Gallery = new Gallery();

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


            CommandStartRecording = new RelayCommand(StartRecording, CanStartRecording);
            CommandStopRecording = new RelayCommand(StopRecording, CanStopRecording);
            CommandShowGraph = new RelayCommand(ShowGraph, CanShowGraph);



        }

       


        //start the timer for polling
        private void ResetGraphTimer()
        {
            GraphTimer.Stop();
            GraphTimer.Start();
        }

        //event that runs every milisecondinterval
        private void TimerEvent(object sender, EventArgs e)
        {
            string val = Connection.ReadFromPort();
            Console.WriteLine(val);

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

            //convert ibi value to heartrate
            if (ibiValue > 0)
            {
                Heartrate = (60000 / ibiValue); //http://www.psylab.com/html/default_heartrat.htm
                ChangeProperty("CurrentHeartbeat");
            }
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