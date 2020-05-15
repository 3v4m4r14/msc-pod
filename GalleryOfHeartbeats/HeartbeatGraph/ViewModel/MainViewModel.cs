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

        private bool noSensorMode = false;
        private Random rnd = new Random();

        //max amount of datapoints able to be shown at once
        private int maxPointsShownAtOnce = 20;
        public int MaxPointsShownAtOnce
        {
            get
            {
                return maxPointsShownAtOnce;
            }
            set
            {
                maxPointsShownAtOnce = value;
                RaisePropertyChanged("MaxPointsShownAtOnce");
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

        public string CurrentHeartbeat
        {
            get
            {
                return "heartrate: " + heartrate;
            }
        }

        

        //all datapoints
        private IList<DataPoint> allPoints; 
        public IList<DataPoint> AllPoints
        {
            get
            {
                return allPoints;
            }
            set
            {
                allPoints = value;
            }
        }

        //model for the graph
        private PlotModel graphModel = new PlotModel();
        public PlotModel GraphModel
        {
            get
            {
                return graphModel;
            }
            set
            {
                graphModel = value;
                RaisePropertyChanged("GraphModel");
            }
        }

        public bool StartBtnIsEnabled = true;
        public bool StopButtonIsEnabled = false;

        public ICommand CommandStartRecording { get; private set; }
        private void StartRecording()
        {
            Console.WriteLine("Start");

            
   
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
            
            

            CommandStartRecording = new RelayCommand(StartRecording);
            CommandStopRecording = new RelayCommand(StopRecording);
            //
            //Options = new ObservableCollection<string>();
            //graphModel.Title = "Heartrate";
            //graphModel.Series.Add(lines);
            //
            ////setup the y-axis
            //graphModel.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = 150, MajorStep = 20, MinorStep = 5 });
            //
            ////starts the graph at 0 it's not nessasary but i just like it
            //allPoints = new List<DataPoint>
            //{
            //    new DataPoint(0,0)
            //};
            
            //if (noSensorMode)
            //{
            //    InitTimer();
            //}
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
            //check if serialport is usable
            if (mySerialPort != null)
            {
                if (mySerialPort.IsOpen)
                {
                    //read serialport
                    byte[] output = new byte[mySerialPort.BytesToRead];
                    mySerialPort.Read(output, 0, output.Length);
                    string val = Encoding.UTF8.GetString(output, 0, output.Length);
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
                }
            }
            else
            {
                //random data for nosensormode
                heartrate = rnd.Next(60, 80);
            }
            //get x point
            time += (float)milisecondInterval / 1000;
            //add points to graph
            AddPoint(time, heartrate); 
        }

        //add point to graph
        private void AddPoint(float x ,int pointValue)
        {
            AllPoints.Add(new DataPoint(x, pointValue));
            lines.Points.Clear();

            if (allPoints.Count > maxPointsShownAtOnce)
            {
                for (int i = AllPoints.Count - maxPointsShownAtOnce; i < allPoints.Count; i++)
                {
                    lines.Points.Add(allPoints[i]); 
                }
            }
            else
            {
                for (int i = 0; i < allPoints.Count; i++)
                {
                    lines.Points.Add(allPoints[i]);;
                }
            }
            graphModel.InvalidatePlot(true);
        }
    }
}