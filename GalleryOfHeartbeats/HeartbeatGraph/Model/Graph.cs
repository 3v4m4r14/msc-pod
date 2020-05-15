using OxyPlot;
using OxyPlot.Axes;
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
        private static readonly int MAX_DATA_POINTS_SHOWN_AT_ONCE = 200;

        OxyPlot.Series.LineSeries lines = new OxyPlot.Series.LineSeries();

        public Graph(string title)
        {
            graphModel.Title = title;
            graphModel.Series.Add(lines);
            graphModel.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = 150, MajorStep = 20, MinorStep = 5 });

            allPoints = new List<DataPoint> { new DataPoint(0,0) };
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
                ChangeProperty("GraphModel");
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

        //add point to graph
        public void AddPoint(float x, int pointValue)
        {
            AllPoints.Add(new DataPoint(x, pointValue));
            lines.Points.Clear();

            if (allPoints.Count > MAX_DATA_POINTS_SHOWN_AT_ONCE)
            {
                for (int i = AllPoints.Count - MAX_DATA_POINTS_SHOWN_AT_ONCE; i < allPoints.Count; i++)
                {
                    lines.Points.Add(allPoints[i]);
                }
            }
            else
            {
                for (int i = 0; i < allPoints.Count; i++)
                {
                    lines.Points.Add(allPoints[i]); ;
                }
            }
            graphModel.InvalidatePlot(true);
        }


        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void ChangeProperty(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
