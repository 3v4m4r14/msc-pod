////////////////////////////////////////////
/////////Don't look at my garbage!!/////////
///////////Author: Lars Hulsmans////////////
////////////////////////////////////////////


using GalaSoft.MvvmLight;
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
using GalaSoft.MvvmLight.Command;
using GalleryOfHeartbeats.Model;

namespace GalleryOfHeartbeats.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private SerialPort mySerialPort;

        

        public int heartrate;
        private float time = 0.0f;
        private int milisecondInterval = 500;

        private Random rnd = new Random();

        public string CurrentHeartbeat
        {
            get
            {
                return "heartrate: " + heartrate;
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

        public bool StartBtnIsEnabled = true;
        public bool StopButtonIsEnabled = false;

        public ICommand CommandStartRecording { get; private set; }
        private void StartRecording()
        {
            Console.WriteLine("Start");

            InitTimer();
   
        }

        public ICommand CommandStopRecording { get; private set; }
        private void StopRecording()
        {
            Console.WriteLine("Stop");

        }


        OxyPlot.Series.LineSeries lines = new OxyPlot.Series.LineSeries();

        public MainViewModel()
        {
            Connection = new Connection();
            Graph = new Graph("Heartrate");
            

            CommandStartRecording = new RelayCommand(StartRecording);
            CommandStopRecording = new RelayCommand(StopRecording);
            
            
            
        }

       


        //start the timer for polling
        private void InitTimer()
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(timer1_Tick);
            aTimer.Interval = milisecondInterval;
            aTimer.Enabled = true;
        }

        //event that runs every milisecondinterval
        private void timer1_Tick(object sender, EventArgs e)
        {
            string val = Connection.ReadFromPort();
            Console.WriteLine(val);
            if (!string.IsNullOrEmpty(val))
            {
                //get first value
                int ibiValue = 0;
                string firstval = "";
                for (int j = 0; j < val.Length; j++)
                {
                    if(val[j] != '\n')
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
                    heartrate = (60000 / ibiValue);
                    RaisePropertyChanged("CurrentHeartbeat");
                }
            }
            else
            {
                //random data for nosensormode
                heartrate = 0;
            }
            //get x point
            time += (float)milisecondInterval / 1000;
            //add points to graph
            Graph.AddPoint(time, heartrate); 
        }
    }
}