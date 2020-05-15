using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryOfHeartbeats.Model
{
    public class Graph : INotifyPropertyChanged
    {
        private static readonly int MAX_DATA_POINTS_SHOWN_AT_ONCE = 20;

        public Graph()
        {

        }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}
