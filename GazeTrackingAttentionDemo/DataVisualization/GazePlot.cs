using GazeTrackingAttentionDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GazeTrackingAttentionDemo.DataVisualization
{
	class GazePlot
	{
		double startTime;
		double endTime;

		List<Fixation> fixations;
		List<Tuple<Ellipse, TextBlock>> fixationPoints;
		Canvas c;

		
		public GazePlot(List<Fixation> fixations, Canvas c) //TODO add gaze stream data
		{
			this.fixations = fixations;
			fixationPoints = new List<Tuple<Ellipse, TextBlock>>();
			this.c = c;

			int i = 0;
			foreach (Fixation f in fixations)
			{
				Ellipse e = new Ellipse();
				e.Fill = new SolidColorBrush(Color.FromArgb(200, 0, 0, 204));
				e.Stroke = Brushes.Black;
				e.Width = e.Height = Math.Sqrt(f.duration * 25 / Math.PI);

				Canvas.SetLeft(e, f.centroid.X - (e.Width / 2));
				Canvas.SetTop(e, f.centroid.Y - (e.Height / 2));

				TextBlock t = new TextBlock();
				t.Text = "" + i;
				t.Foreground = new SolidColorBrush(Colors.White);
				t.Height = 25;
				t.Width = 25;
				t.TextAlignment = TextAlignment.Center;
				t.VerticalAlignment = VerticalAlignment.Center;
				t.FontSize = 20;
				t.FontWeight = FontWeights.Bold;
				//t.Background = new SolidColorBrush(Colors.Red);

				Canvas.SetLeft(t, f.centroid.X - (e.Width / 2));
				Canvas.SetTop(t, f.centroid.Y - (e.Height / 2));

				fixationPoints.Add(Tuple.Create(e,t));

				i++;

				//    e.Fill = mySolidColorBrush;
			}
		}

		public void renderPlot(bool showAll, bool showFixations, bool showSaccades, bool showGaze, double startRange, double endRange)
		{
			foreach(Tuple<Ellipse, TextBlock> t in fixationPoints)
			{
				c.Children.Add(t.Item1);
				c.Children.Add(t.Item2);
			}
		}
	}
}

//private void renderEllipse(Point p, Color c, int num, double radius)
	//{
	//    //draw ellipse
	//    Ellipse e = new Ellipse();

//    SolidColorBrush mySolidColorBrush = new SolidColorBrush(c);

//    e.Fill = mySolidColorBrush;

//    //myEllipse.StrokeThickness = 2;

//    e.Stroke = Brushes.Black;

//    e.Width = radius;

//    e.Height = radius;


//    Canvas.SetLeft(e, p.X - (e.Width / 2));
//    Canvas.SetTop(e, p.Y - (e.Height / 2));

//    mainCanvas.Children.Add(e);

//    //add number
//    TextBlock t = new TextBlock();
//    t.Text = "" + num;
//    t.Foreground = new SolidColorBrush(Colors.White);
//    t.Height = 25;
//    t.Width = 25;
//    t.TextAlignment = TextAlignment.Center;
//    t.VerticalAlignment = VerticalAlignment.Center;
//    t.FontSize = 20;
//    t.FontWeight = FontWeights.Bold;
//    //t.Background = new SolidColorBrush(Colors.Red);

//    Canvas.SetLeft(t, p.X - (t.Width / 2));
//    Canvas.SetTop(t, p.Y - (t.Height / 2));

//    mainCanvas.Children.Add(t);
//}
