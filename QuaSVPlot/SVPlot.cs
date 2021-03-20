using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Series;
using Quaver.API;
using Quaver.API.Maps;
using Quaver.API.Maps.Structures;
using System;

namespace QuaSVPlot
{
    public class SVPlot
    {
        public PlotModel Model { get; private set; }
        
        public Qua Map { get; private set; }
        
        public SVPlot()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".qua";
            dialog.Filter = "Quaver map files (.qua)|*.qua";
            dialog.InitialDirectory = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Quaver\\Songs";
            
            bool? result = dialog.ShowDialog();
            
            if (result == true)
                Map = Qua.Parse(dialog.FileName, false);
            
            Model = new PlotModel { Title = "yes" };
            var SVSeries = new LineSeries();
            SVSeries.ItemsSource = Map.SliderVelocities;
            SVSeries.Mapping = item => new DataPoint(((SliderVelocityInfo)item).StartTime, ((SliderVelocityInfo)item).Multiplier);
            SVSeries.CanTrackerInterpolatePoints = false;
            SVSeries.DataFieldX = "Time";
            SVSeries.DataFieldY = "Multiplier"; 
            Model.Series.Add(SVSeries);
        }
    }
}