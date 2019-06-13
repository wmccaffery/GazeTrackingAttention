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
	public class GazePlot
	{
		double startTime;
		double endTime;

		List<Fixation> fixations;
		List<Saccade> saccades;
		List<Tuple<Ellipse, TextBlock, float, float>> fixationPoints;
		List<Tuple<Line, float, float>> saccadeLines;
		Canvas c;

		private MainWindow _mainWin = (MainWindow) System.Windows.Application.Current.MainWindow;

		public GazePlot(List<Fixation> fixations, List<Saccade> saccades, Canvas c) //TODO add gaze stream data
		{
			this.fixations = fixations;
			fixationPoints = new List<Tuple<Ellipse, TextBlock, float, float>>();
			saccadeLines = new List<Tuple<Line, float, float>>();


			this.c = c;

			foreach (Fixation f in fixations)
			{

				//draw ellipse
				Ellipse e = new Ellipse();
				e.Fill = new SolidColorBrush(Color.FromArgb(200, 0, 0, 204));
				e.Stroke = Brushes.Black;
				e.Width = e.Height = (Math.Sqrt(f.duration/ Math.PI) * 5) + 25; //scale and add width of text block to radius

				//get window coordinates for rendering point
				Point rp = _mainWin.MainCanvas.PointFromScreen(f.centroid);
				//Point rp = f.centroid;


				//set ellipse position
				Canvas.SetLeft(e, rp.X - (e.Width / 2));
				Canvas.SetTop(e, rp.Y - (e.Height / 2));

				//draw number on ellipse
				TextBlock t = new TextBlock();
				t.Text = "" + f.id;
				t.Foreground = new SolidColorBrush(Colors.White);
				t.Height = 25;
				t.Width = 25;
				t.TextAlignment = TextAlignment.Center;
				t.VerticalAlignment = VerticalAlignment.Center;
				t.FontSize = 20;
				t.FontWeight = FontWeights.Bold;
				
				//t.Background = new SolidColorBrush(Colors.Red);
				
				//set text position
				Canvas.SetLeft(t, rp.X - (t.Width / 2));
				Canvas.SetTop(t, rp.Y - (t.Height / 2));

				//add to list to be rendered
				fixationPoints.Add(Tuple.Create(e,t, f.startPos.timestamp, f.endPos.timestamp));
				//    e.Fill = mySolidColorBrush;
			}

			foreach(Saccade s in saccades)
			{
				Line l = new Line();
				Point spos = _mainWin.MainCanvas.PointFromScreen(s.start.centroid);
				Point epos = _mainWin.MainCanvas.PointFromScreen(s.finish.centroid);

				l.X1 = spos.X;
				l.X2 = epos.X;
				l.Y1 = spos.Y;
				l.Y2 = epos.Y;
				l.Stroke = Brushes.Black;

				saccadeLines.Add(Tuple.Create(l, s.start.endPos.timestamp, s.finish.endPos.timestamp));
			}
		}

		public void renderPlot(bool showAll, bool showFixations, bool showSaccades, double startRange, double endRange)
		{
			c.Children.Clear();
			if (showFixations) {
				foreach (Tuple<Ellipse, TextBlock, float, float> t in fixationPoints)
				{
					Console.WriteLine(t.Item4 + " TO END " + endRange );
					if (t.Item4 > endRange || t.Item3 < startRange)
					{
						t.Item1.Fill = new SolidColorBrush(Color.FromArgb(100, 211, 211, 211));
					} else
					{
						t.Item1.Fill = new SolidColorBrush(Color.FromArgb(200, 0, 0, 204));
					}
					c.Children.Add(t.Item1);
					c.Children.Add(t.Item2);
				}
			}

			if (showSaccades)
			{
				foreach(Tuple<Line, float, float> t in saccadeLines)
				{
					c.Children.Add(t.Item1);
				}
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
