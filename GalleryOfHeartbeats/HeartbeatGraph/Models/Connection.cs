// Gallery of Heartbeats
// Author: Eva Maria Veitmaa
// Date: 2020

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryOfHeartbeats.Model
{
    public class Connection
    {

        private SerialPort mySerialPort;

        public Connection()
        {
            Options = new ObservableCollection<string>();
            GetComPorts();
        }

        public Connection(bool connectAutomatically)
        {
            Options = new ObservableCollection<string>();
            GetComPorts();

            if (connectAutomatically)
            {
                foreach (string _port in Options)
                {
                    if (ConnectToPort(_port)) { break; }
                }
            }
        }

        #region Options
        public ObservableCollection<string> Options { get; set; }
        #endregion

        #region SelectedPort
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
            }
        }
        #endregion

        private void OnPortChange()
        {
            //check if port is available
            if (ConnectToSelectedPort())
            {
                Console.WriteLine("Connected to port " + selectedPort);
            }
            else
            {
                Console.WriteLine("Could not connect to port " + selectedPort);
            }
        }


        //get a list of all the available comports
        private void GetComPorts()
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                Options.Add(port);
                Console.WriteLine(port);
            }
        }

        public bool PortIsReady()
        {
            return mySerialPort != null && mySerialPort.IsOpen;
        }

        private bool ConnectToPort(string _port)
        {
            if (mySerialPort != null && !mySerialPort.IsOpen) {
            try
            {
                mySerialPort = new SerialPort(_port, 115200, Parity.None, 8, StopBits.One);

                mySerialPort.Handshake = Handshake.None;
                mySerialPort.RtsEnable = false;

                if (!mySerialPort.IsOpen)
                {
                    Console.WriteLine("Port could be connected to!" + _port);
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
            return false;
        }

        private bool ConnectToSelectedPort()
        {
            try
            {
                mySerialPort = new SerialPort(selectedPort, 115200, Parity.None, 8, StopBits.One);

                mySerialPort.Handshake = Handshake.None;
                mySerialPort.RtsEnable = false;

                if (!mySerialPort.IsOpen)
                {
                    Console.WriteLine("Port could be connected to!");
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

        public string ReadFromPort()
        {
            if(PortIsReady())
            { 
                //read serialport
                byte[] output = new byte[mySerialPort.BytesToRead];
                mySerialPort.Read(output, 0, output.Length);
                string val = Encoding.UTF8.GetString(output, 0, output.Length);
                return val;
            }
            return "";
        }
    }
}
