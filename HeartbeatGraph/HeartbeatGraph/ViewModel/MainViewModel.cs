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
using SimpleTCP;

namespace HeartbeatGraph.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private SerialPort mySerialPort;

        SimpleTcpClient Client;

        public int ibiValue;
        public int curHeartrate;
        public int prevHeartrate;
        float intensity;
        private float time = 0.0f;
        private int milisecondInterval = 5;

        private bool noSensorMode = false;
        private Random rnd = new Random();

        //max amount of datapoints able to be shown at once
        private int maxPointsShownAtOnce = 200;
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

        //list of the available com ports
        private ObservableCollection<string> options;
        public ObservableCollection<string> Options
        {
            get
            {
                return options;
            }
            set
            {
                options = value;
                RaisePropertyChanged("Options");
            }
        }

        public string CurrentHeartbeat
        {
            get
            {
                return "heartrate: " + curHeartrate;
            }
        }

        //selected com port
        private string selectedPort = "";
        public string SelectedPort
        {
            get
            {
                return selectedPort;
            }
            set
            {
                selectedPort = value;
                OnPortChange();
                RaisePropertyChanged("SelectedPort");
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

        OxyPlot.Series.LineSeries lines = new OxyPlot.Series.LineSeries();

        public MainViewModel()
        {
            Client = new SimpleTcpClient().Connect("127.0.0.1", 3000);
            Options = new ObservableCollection<string>();
            //graphModel.Title = "Heartrate";
            //graphModel.Series.Add(lines);
            //
            ////setup the y-axis
            //graphModel.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = 150, MajorStep = 20, MinorStep = 5 });
            
            //starts the graph at 0 it's not nessasary but i just like it
            //allPoints = new List<DataPoint>
            //{
            //    new DataPoint(0,0)
            //};
            //get available comports
            GetComPorts();
            if (noSensorMode)
            {
                InitTimer();
            }
        }

        private void OnPortChange()
        {
            //check if port is available
            if (ConnectToPort())
            {
                InitTimer();
            }
        }

        //connect to the serial port
        private bool ConnectToPort()
        {
            try
            {
                mySerialPort = new SerialPort(selectedPort, 115200, Parity.None, 8, StopBits.One);

                mySerialPort.Handshake = Handshake.None;
                mySerialPort.RtsEnable = false;

                if (!mySerialPort.IsOpen)
                {
                    mySerialPort.Open();
                    return true;
                }
                return false;
            }
            catch
            {
                Console.WriteLine(string.Format("a connection to {0} could not be made", selectedPort));
                return false;
            }
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
                        ibiValue = 0;
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
                        //convert ibi value to curHeartrate
                        if (ibiValue > 0)
                        {
                            curHeartrate = (60000 / ibiValue);
                            RaisePropertyChanged("CurrentHeartbeat");
                        }
                    }
                }
            }
            else
            {
                //random data for nosensormode
                curHeartrate = rnd.Next(60, 80);
            }

            ControlPod(curHeartrate);

            //get x point
            time += (float)milisecondInterval / 1000;
            //add points to graph
            //AddPoint(time, curHeartrate); 
        }

        private void ControlPod(int currentHeartrate)
        {
            if (prevHeartrate < curHeartrate)
            {
                intensity = 0.2f;

                prevHeartrate = curHeartrate;
                Console.WriteLine(intensity);
            }
            else if (prevHeartrate > currentHeartrate)
            {
                intensity = 0;

                prevHeartrate = curHeartrate;
                Console.WriteLine(intensity);
            }

            Client.WriteLine(String.Format("SetLightColor|LEFT|{0}|{1}|{2}|{3}", intensity, intensity, intensity, intensity));
            Client.WriteLine(String.Format("SetLightColor|RIGHT|{0}|{1}|{2}|{3}", intensity, intensity, intensity, intensity));

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

        //get a list of all the available comports
        private void GetComPorts()
        {
            string[] ports = SerialPort.GetPortNames();
            foreach(string port in ports)
            {
                Options.Add(port);
            }
        }
    }
}