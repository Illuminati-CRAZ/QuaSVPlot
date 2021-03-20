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
        
        public LineSeries PositionSeries { get; private set; }
        
        public LineSeries DisplacementSeries { get; private set; }
        
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
                
            Map.NormalizeSVs();
            
            // Plot stuff
            Model = new PlotModel { Title = Map.ToString() };
            Model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Key = "Time", Title = "Time (ms)" });
            Model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Key = "Velocity", Title = "Velocity", Minimum = -10, Maximum = 10});
            Model.Axes.Add(new LinearAxis { Position = AxisPosition.Right, Key = "Position", Title = "Position", PositionTier = 1 });
            
            // Scroll Velocities
            SVSeries = new StairStepSeries();
            SVSeries.Title = "Scroll Velocities";
            SVSeries.ItemsSource = Map.SliderVelocities;
            SVSeries.Mapping = item => new DataPoint(((SliderVelocityInfo)item).StartTime, ((SliderVelocityInfo)item).Multiplier);
            SVSeries.VerticalStrokeThickness = .25;
            SVSeries.CanTrackerInterpolatePoints = false;
            SVSeries.DataFieldX = "Time";
            SVSeries.DataFieldY = "Multiplier";
            SVSeries.YAxisKey = "Velocity";
            
            // Position
            PositionSeries = new LineSeries();
            PositionSeries.Title = "Position";
            PositionSeries.CanTrackerInterpolatePoints = true;
            PositionSeries.DataFieldX = "Time";
            PositionSeries.DataFieldY = "Position";
            PositionSeries.YAxisKey = "Position";
            
            // Displacement
            DisplacementSeries = new LineSeries();
            DisplacementSeries.Title = "Displacement";
            DisplacementSeries.CanTrackerInterpolatePoints = true;
            DisplacementSeries.DataFieldX = "Time";
            DisplacementSeries.DataFieldY = "Displacement";
            DisplacementSeries.YAxisKey = "Position";
            
            // Calculate position and displacement data points based off of Quaver's position calculations
            // This is pretty much just HitObjectManagerKeys.InitializePositionMarkers()
            long position = (long)(Map.SliderVelocities[0].StartTime * Map.InitialScrollVelocity * 100);
            PositionSeries.Points.Add(new DataPoint(Map.SliderVelocities[0].StartTime, position));
            for (int i = 1; i < Map.SliderVelocities.Count; i++)
            {
                position += (long)((Map.SliderVelocities[i].StartTime - Map.SliderVelocities[i - 1].StartTime)
                                    * Map.SliderVelocities[i - 1].Multiplier * 100);
                PositionSeries.Points.Add(new DataPoint(Map.SliderVelocities[i].StartTime, position));
                
                // Displacement function = Position function - Base position function (constant 1x SV)
                DisplacementSeries.Points.Add(new DataPoint(Map.SliderVelocities[i].StartTime, position - Map.SliderVelocities[i].StartTime * 100));
            }
            
            Model.Series.Add(SVSeries);
            Model.Series.Add(PositionSeries);
            Model.Series.Add(DisplacementSeries);
        }
    }
}