// Gallery of Heartbeats
// Author: Eva Maria Veitmaa
// Date: 2020

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryOfHeartbeats.Model
{
    class GalleryItem
    {
        public string Name { get; set; }
        public string TimeOfRecording { get; set; }
        public int PollingRate { get; set; }
        public List<int> Data { get; set; }
        public int Progress { get; set; } = 10;

        override public string ToString()
        {
            string dataAsString = "";
            foreach(int value in Data)
            {
                dataAsString += value + " ";
            }

            return "Name: " + Name + "\nTimeOfRecording: " + TimeOfRecording + "\nPollingRate: " + PollingRate + "\nData: " + dataAsString;
        }
    }
}
