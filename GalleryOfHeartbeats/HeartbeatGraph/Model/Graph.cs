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
    public class Graph
    {
        private static readonly int MAX_DATA_POINTS_SHOWN_AT_ONCE = 200;

        OxyPlot.Series.LineSeries lines;

        public Graph(string title)
        {
            GraphModel.Title = title;

            lines = new OxyPlot.Series.LineSeries();
            GraphModel.Series.Add(lines);

            GraphModel.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = 150, MajorStep = 20, MinorStep = 5 });

            AllPoints = new List<DataPoint> { new DataPoint(0,0) };
        }

        public PlotModel GraphModel { get; set; } = new PlotModel();
        public IList<DataPoint> AllPoints { get; set; }

        public void ResetGraph()
        {
            GraphModel.Series.Clear();
            lines.Points.Clear();
            GraphModel.Series.Add(lines);

            AllPoints = new List<DataPoint> { new DataPoint(0, 0) };

            GraphModel.InvalidatePlot(true);
        }

        //add point to graph
        public void AddPoint(float x, int pointValue)
        {
            AllPoints.Add(new DataPoint(x, pointValue));
            lines.Points.Clear();

            if (AllPoints.Count > MAX_DATA_POINTS_SHOWN_AT_ONCE)
            {
                for (int i = AllPoints.Count - MAX_DATA_POINTS_SHOWN_AT_ONCE; i < AllPoints.Count; i++)
                {
                    lines.Points.Add(AllPoints[i]);
                }
            }
            else
            {
                for (int i = 0; i < AllPoints.Count; i++)
                {
                    lines.Points.Add(AllPoints[i]); ;
                }
            }
            GraphModel.InvalidatePlot(true);
        }
    }
}
