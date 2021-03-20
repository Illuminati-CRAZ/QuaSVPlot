using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
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
        
        public StairStepSeries SVSeries { get; private set; }
        
        public Qua Map { get; private set; }
        
        public SVPlot()
        {
            // Select .qua file
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".qua";
            dialog.Filter = "Quaver map files (.qua)|*.qua";
            dialog.InitialDirectory = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Quaver\\Songs";
            
            bool? result = dialog.ShowDialog();
            
            if (result == true)
                Map = Qua.Parse(dialog.FileName, false);
            
            // Plot stuff
            Model = new PlotModel { Title = Map.ToString() };
            Model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
            Model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = -10, Maximum = 10});
            
            SVSeries = new StairStepSeries();
            SVSeries.Title = "Scroll Velocities";
            SVSeries.ItemsSource = Map.SliderVelocities;
            SVSeries.Mapping = item => new DataPoint(((SliderVelocityInfo)item).StartTime, ((SliderVelocityInfo)item).Multiplier);
            SVSeries.VerticalStrokeThickness = .25;
            SVSeries.CanTrackerInterpolatePoints = false;
            SVSeries.DataFieldX = "Time";
            SVSeries.DataFieldY = "Multiplier"; 
            
            // Generate Data Points
            /*foreach (SliderVelocityInfo sv in Map.SliderVelocities)
            {
                SVSeries.Points.Add(new DataPoint(sv.StartTime, sv.Multiplier));
            }*/
            
            Model.Series.Add(SVSeries);
        }
    }
}