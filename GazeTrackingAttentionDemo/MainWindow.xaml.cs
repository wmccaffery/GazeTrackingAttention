using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tobii.EyeX;
using Tobii.EyeX.Framework;
using Tobii.Interaction;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace GazeTrackingAttentionDemo
{
    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataReader fd;
        Boolean test;
        Boolean showGaze;
        Boolean showFixations;
        Boolean showSaccades;
        double displayTime;
        



        public MainWindow()
        {
            InitializeComponent();
            //fd = new DataReader();
            //State.fixations = new ArrayList();
            //mainCanvas.Children.Clear();
            init();
            
        }

        public void init()
        {
            fd = new DataReader();
            State.fixations = new ArrayList();
            State.gazePoints = new ArrayList();
            State.lines = new ArrayList();
            State.startTime = -1;
            State.endTime = -1;
            Render.IsEnabled = false;
            Begin.IsEnabled = true;
            End.IsEnabled = false;
            Test_Callibration.IsEnabled = true;
            Callibrate.IsEnabled = true;
            Reset.IsEnabled = true;
            mainCanvas.Children.Clear();
            DocumentArea.Visibility = Visibility.Visible;
            DisplaySlider.Visibility = Visibility.Hidden;
            DisplaySlider_Value.Visibility = Visibility.Hidden;
            All_CheckBox.Visibility = Visibility.Hidden;
            Saccade_CheckBox.Visibility = Visibility.Hidden;
            Fixation_CheckBox.Visibility = Visibility.Hidden;
            Gaze_CheckBox.Visibility = Visibility.Hidden;
            Visualisation_Header.Visibility = Visibility.Hidden;


            test = false;

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void Test_Click(object sender, RoutedEventArgs e)
        {
            if (test)
            {
                test = false;
                init();
            }
            else
            {
                test = true;
            }
            render();

            DocumentArea.Visibility = Visibility.Hidden;
  
        }

        private void renderTestTargets()
        {
            UIElement container = VisualTreeHelper.GetParent(DocumentArea) as UIElement;
            Point viewerSource = DocumentArea.TranslatePoint(new Point(0, 0), container);

            Point s1 = viewerSource;
            Point s2 = new Point(viewerSource.X, viewerSource.Y + DocumentArea.Height);
            Point s3 = new Point(viewerSource.X, viewerSource.Y + DocumentArea.Height / 2);
            Point s4 = new Point(viewerSource.X, viewerSource.Y + DocumentArea.Height / 4);
            Point s5 = new Point(viewerSource.X, viewerSource.Y + DocumentArea.Height / 2 + DocumentArea.Height / 4);

            Point s6 = new Point(viewerSource.X + DocumentArea.Width / 2, viewerSource.Y);
            Point s7 = new Point(viewerSource.X + DocumentArea.Width / 2, viewerSource.Y + DocumentArea.Height / 2);
            Point s8 = new Point(viewerSource.X + DocumentArea.Width / 2, viewerSource.Y + DocumentArea.Height / 4);
            Point s9 = new Point(viewerSource.X + DocumentArea.Width / 2, viewerSource.Y + DocumentArea.Height / 2 + DocumentArea.Height / 4);
            Point s10 = new Point(viewerSource.X + DocumentArea.Width / 2, viewerSource.Y + DocumentArea.Height);

            Point s11 = new Point(viewerSource.X + DocumentArea.Width, viewerSource.Y);
            Point s12 = new Point(viewerSource.X + DocumentArea.Width, viewerSource.Y + DocumentArea.Height / 2);
            Point s13 = new Point(viewerSource.X + DocumentArea.Width, viewerSource.Y + DocumentArea.Height / 4);
            Point s14 = new Point(viewerSource.X + DocumentArea.Width, viewerSource.Y + DocumentArea.Height / 2 + DocumentArea.Height / 4);
            Point s15 = new Point(viewerSource.X + DocumentArea.Width, viewerSource.Y + DocumentArea.Height);


            drawTestTarget(s1);
            drawTestTarget(s2);
            drawTestTarget(s3);
            drawTestTarget(s4);
            drawTestTarget(s5);
            drawTestTarget(s6);
            drawTestTarget(s7);
            drawTestTarget(s8);
            drawTestTarget(s9);
            drawTestTarget(s10);
            drawTestTarget(s11);
            drawTestTarget(s12);
            drawTestTarget(s13);
            drawTestTarget(s14);
            drawTestTarget(s15);

        }
        private void Begin_Click(object sender, RoutedEventArgs e)
        {
            //fd = new DataReader();
            fd.getEyeTrackingData();
            fd.getEyeTrackingGazePoints();
            End.IsEnabled = true;
            //Begin.Visibility = Visibility.Collapsed;
        }

        private void End_Click(object sender, RoutedEventArgs e)
        {
            fd.Dispose();
            Render.IsEnabled = true;
            Begin.IsEnabled = false;
            End.IsEnabled = false;
            Test_Callibration.IsEnabled = false;
            Callibrate.IsEnabled = false;
            DisplaySlider.Maximum = State.endTime;
            DisplaySlider.Minimum = State.startTime;

        }

        private void Callibrate_Click(object sender, RoutedEventArgs e)
        {
            fd.callibrate();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            init();
        }

        private void Render_Click(object sender, RoutedEventArgs e)
        {
            DisplaySlider.Value = DisplaySlider.Maximum;
            DisplaySlider.Visibility = Visibility.Visible;
            DisplaySlider_Value.Visibility = Visibility.Visible;
            All_CheckBox.Visibility = Visibility.Visible;
            Saccade_CheckBox.Visibility = Visibility.Visible;
            Fixation_CheckBox.Visibility = Visibility.Visible;
            Gaze_CheckBox.Visibility = Visibility.Visible;
            Visualisation_Header.Visibility = Visibility.Visible;
            render();
        }

        private void render()
        {
            mainCanvas.Children.Clear();
            //if (State.fixations.Count == 0)
            //{
            //    Console.WriteLine("nothing to render");
            //}
            if (test)
            {
                renderTestTargets();
            }

            if (showFixations) { 
                for (int i = 0; i < State.fixations.Count; i++)
                {

                    Fixation f = (Fixation)State.fixations[i];
                    if (f.startPoint.timeStamp <= displayTime)
                    {
                        renderEllipse(this.PointFromScreen(f.centroid.toPoint()), Color.FromArgb(127, Colors.Blue.R, Colors.Blue.G, Colors.Blue.B), i, 25 + (f.duration / 4));
                    }
                }
            }

            if (showSaccades)
            {
                foreach (Saccade l in State.lines)
                {
                    {
                        if (l.start <= displayTime) ;
                        renderLine(l, Colors.Black);
                    }
                }
            }

            if (showGaze)
            {
                foreach (DataPoint p in State.gazePoints)
                {
                    if (p.timeStamp <= displayTime)
                    {
                        //renderEllipse(p.toPoint(), Colors.Red, 5);
                        Point pt = this.PointFromScreen(p.toPoint());
                        renderEllipse(pt, Colors.Gold, 5);
                    }

                }
            }



        }

        private void renderEllipse(Point p, Color c, int num,  double radius)
        {
            //draw ellipse
            Ellipse e = new Ellipse();

            SolidColorBrush mySolidColorBrush = new SolidColorBrush(c);

            e.Fill = mySolidColorBrush;

            //myEllipse.StrokeThickness = 2;

            e.Stroke = Brushes.Black;

            e.Width = radius;

            e.Height = radius;


            Canvas.SetLeft(e, p.X - (e.Width / 2));
            Canvas.SetTop(e, p.Y - (e.Height / 2));

            mainCanvas.Children.Add(e);

            //add number
            TextBlock t = new TextBlock();
            t.Text = "" + num;
            t.Foreground = new SolidColorBrush(Colors.White);
            t.Height = 25;
            t.Width = 25;
            t.TextAlignment = TextAlignment.Center;
            t.VerticalAlignment = VerticalAlignment.Center;
            t.FontSize = 20;
            t.FontWeight = FontWeights.Bold;
            //t.Background = new SolidColorBrush(Colors.Red);

            Canvas.SetLeft(t, p.X - (t.Width / 2));
            Canvas.SetTop(t, p.Y - (t.Height / 2));

            mainCanvas.Children.Add(t);
        }

        private void renderEllipse(Point p, Color c, double radius)
        {
            //draw ellipse
            Ellipse e = new Ellipse();

            SolidColorBrush mySolidColorBrush = new SolidColorBrush(c);

            e.Fill = mySolidColorBrush;

            //myEllipse.StrokeThickness = 2;

            e.Stroke = Brushes.Black;

            e.Width = radius;

            e.Height = radius;

            Canvas.SetLeft(e, p.X - (e.Width / 2));
            Canvas.SetTop(e, p.Y - (e.Height / 2));

            mainCanvas.Children.Add(e);
        }

        private void drawTestTarget(Point p)
        {
            //add number
            Ellipse r = new Ellipse();
            r.Height = 10;
            r.Width = 10;

            SolidColorBrush mySolidColorBrush = new SolidColorBrush(Colors.Black);

            r.Fill = mySolidColorBrush;

            //myEllipse.StrokeThickness = 2;

            Canvas.SetLeft(r, p.X - (r.Width / 2));
            Canvas.SetTop(r, p.Y - (r.Height / 2));

            mainCanvas.Children.Add(r);

            Ellipse r2 = new Ellipse();
            r2.Height = 50;
            r2.Width = 50;

            SolidColorBrush mySolidColorBrush2 = new SolidColorBrush(Colors.Transparent);

            r2.Fill = mySolidColorBrush2;

            r2.Stroke = Brushes.Black;

            Canvas.SetLeft(r2, p.X - (r2.Width / 2));
            Canvas.SetTop(r2, p.Y - (r2.Height / 2));

            mainCanvas.Children.Add(r2);
        }

        private void renderLine(Saccade line, Color c)
        {
            try
            {
                Line l = new Line();
                l.X1 = line.X1;
                l.X2 = line.X2;
                l.Y1 = line.Y1;
                l.Y2 = line.Y2;
                l.Stroke = new SolidColorBrush(c);
                l.StrokeThickness = 2;
                mainCanvas.Children.Add(l);
            }
            catch (Exception e)
            {
                Console.WriteLine("TEMP WORKAROUND PLZ FIX");
            }
        }

        private void DisplayTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            displayTime = DisplaySlider.Value;
            render();
        }

        private void Gaze_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            showGaze = true;
            render();
        }

        private void Gaze_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            showGaze = false;
            render();

        }

        private void Fixation_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            showFixations = true;
            render();

        }

        private void Fixation_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            showFixations = false;
            render();

        }

        private void Saccade_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            showSaccades = true;
            render();

        }

        private void Saccade_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            showSaccades = false;
            render();

        }

        private void All_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Saccade_CheckBox.IsChecked = true;
            Fixation_CheckBox.IsChecked = true;
            Gaze_CheckBox.IsChecked = true;
            render();

        }

        private void All_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Saccade_CheckBox.IsChecked = false;
            Fixation_CheckBox.IsChecked = false;
            Gaze_CheckBox.IsChecked = false;
            render();

        }
    }

    public class BuildA4
    {
        double width = 8.27 * (1920 / 20.4);
        double height = 11.69 * (1200 / 12.8);

        public double getA4Width()
        {
            return width;
        }

        public double getA4Height()
        {
            return height;
        }

    }

    public class DataReader : IDisposable
    {
        private readonly Host _host;
        private readonly FixationDataStream _fixationDataStream;
        private readonly GazePointDataStream _gazePointDataStream;
        

        public DataReader()
        {
            _host = new Host();
            _fixationDataStream = _host.Streams.CreateFixationDataStream();
            //_fixationDataStream = _host.Streams.CreateFixationDataStream(Tobii.Interaction.Framework.FixationDataMode.Slow);
            _gazePointDataStream = _host.Streams.CreateGazePointDataStream();
        }

        public void callibrate()
        {
            _host.Context.LaunchConfigurationTool(Tobii.Interaction.Framework.ConfigurationTool.Recalibrate, (data) => { });
        }

        public void getEyeTrackingData()
        {
           

            //store fixation
            Fixation f = new Fixation();
            _fixationDataStream.Begin((x, y, timestamp) => 
            {
                //Console.WriteLine("Begin fixation at X: {0} Y: {1} Timestamp: {2}", x, y, timestamp);
                //DataStore.fixations.Add(new Point(x, y));
                Point p = new Point(x, y);                
                f.startPoint = new DataPoint(x, y, timestamp);
            });
            _fixationDataStream.Data((x, y, timestamp) => 
            {
                //Console.WriteLine("During fixation at X: {0} Y: {1}", x, y);
                f.points.Add(new DataPoint(x, y, timestamp));
            });
            _fixationDataStream.End((x, y, timestamp) => 
            {
                //Console.WriteLine("End fixation at X: {0} Y: {1}", x, y);
                f.endPoint = new DataPoint( x, y, timestamp);
                
                f.centroid = f.getCentroid();
                f.duration = f.endPoint.timeStamp - f.startPoint.timeStamp;
                if (!Double.IsNaN(f.startPoint.rawX) && !Double.IsNaN(f.startPoint.rawY) && !Double.IsNaN(f.endPoint.rawX) && !Double.IsNaN(f.endPoint.rawY))
                {
                    //Console.WriteLine(f.startPoint.X);
                    //Console.WriteLine("duration {0})", f.duration);
                    if (f.startPoint.timeStamp != 0)
                    {
                        State.fixations.Add(f);
                        Console.WriteLine("NEW FIXATION: \n" +
                        "xPos:{0}\n" +
                        "yPos:{1}\n" +
                        "timestamp:{2}\n" +
                        "duration:{3}\n" +
                        "startPoint:{4}\n" +
                        "subPoints:{5}\n" +
                        "endPoint:{6}\n", f.centroid.rawX, f.centroid.rawY, f.startPoint.timeStamp, f.duration, f.startPoint.toString(), f.points.ToString(), f.endPoint.toString());
                        attemptSaccade();
                        f = new Fixation();
                    }
                }
                //f.getLines();

            });
        }

        private void attemptSaccade()
        {
            //store saccade
            if (State.fixations.Count > 1)
            {
                Saccade s = new Saccade();
                Fixation f1 = ((Fixation)State.fixations[State.fixations.Count - 2]);
                Fixation f2 = ((Fixation)State.fixations[State.fixations.Count - 1]);
                s.X1 = f1.centroid.rawX;
                s.Y1 = f1.centroid.rawY;
                s.X2 = f2.centroid.rawX;
                s.Y2 = f2.centroid.rawY;
                s.hlength = s.X2 - s.X1;
                s.vlength = s.Y2 - s.Y1;
                s.size = Math.Sqrt(Math.Pow(s.hlength, 2) + Math.Pow(s.vlength, 2));
                s.start = f1.endPoint.timeStamp;
                s.end = f2.startPoint.timeStamp;
                s.duration = f2.startPoint.timeStamp - f1.endPoint.timeStamp;
                State.lines.Add(s);

                Console.WriteLine("NEW SACCADE: \n" +
                "size:{0}\n" +
                "duration:{1}\n" +
                "startTime:{2}\n" +
                "endTime:{3}\n" +
                "x1Pos:{4}\n" +
                "y1Pos:{5}\n" +
                "x2Pos:{6}\n" +
                "y2Pos:{7}\n", s.size, s.duration, s.start, s.end, s.X1, s.Y1, s.X2, s.Y2);

            }
        }

        public void getEyeTrackingGazePoints()
        {
            
            _gazePointDataStream.GazePoint((x, y, timestamp) =>
            {
                State.gazePoints.Add(new DataPoint(x, y, timestamp));
                if(State.startTime == -1)
                {
                    State.startTime = timestamp; 
                }
                if(timestamp > State.endTime)
                {
                    State.endTime = timestamp;
                }
            });
        }


        public void Dispose()
        {
            if (_host != null)
            {
                try
                {
                    _host.Dispose();
                }
                catch (System.ObjectDisposedException e) 
                {
                    Console.WriteLine("_host was already disposed");
                }
            }
        }
    }

    public static class State
    {
        public static ArrayList Documents = new ArrayList();
        public static Document currentDocument;
        public static Page currentPage;

        public static ArrayList fixations= new ArrayList();
        public static ArrayList gazePoints = new ArrayList();
        public static ArrayList lines = new ArrayList();

        public static double startTime = -1;
        public static double endTime = -1;

        public static double getLastTimestamp()
        {
            return ((Fixation)fixations[fixations.Count - 1]).endPoint.timeStamp;
        }

        public static double getFirstTimestamp()
        {
            return ((Fixation)fixations[0]).startPoint.timeStamp;
        }
    }
    
    public class Document
    {
        public ArrayList pages;
    }

    public class Page
    {
        //fixations recorded on that page
        public ArrayList fixations;
        public ArrayList gazeData;

        //TODO page img/page of pdf to display
        //...

    }

    public class Fixation
    {
        //Point point1;
        //Point point2;

        public DataPoint startPoint;
        public ArrayList points;
        public DataPoint endPoint;
        public ArrayList lines;
        public DataPoint centroid;

        public double duration;

        //move draw function inside here
        public Fixation()
        {
            points = new ArrayList();
            //lines = new ArrayList();
        }

        //public void getLines()
        //{
            //check for existing fixations
            //if (State.fixations.Count > 1)
            //{
            //    point1 = ((Fixation)State.fixations[State.fixations.Count-2]).endPoint;

            //    point2 = startPoint;
            //    lines.Add(new LineWithTimestamp(point1, point2, point2.timeStamp));
            //}

            //point1 = startPoint;

            //foreach(Point p in points)
            //{
            //    point2 = p;
            //    lines.Add(new LineWithTimestamp(point1, point2, point2.timeStamp));

            //    point1 = p;
            //}

            //point2 = endPoint;
            //lines.Add(new LineWithTimestamp(point1, point2, point2.timeStamp));
        //}


        //TODO add weighting using timestamp
        public DataPoint getCentroid()
        {
            double totalX = 0;
            double totalY = 0;
            int count = 0;

            totalX += startPoint.rawX;
            totalY += startPoint.rawY;
            count++;

            foreach (DataPoint p in points)
            {
                totalX += p.rawX;
                totalY += p.rawY;
                count++;
            }

            totalX += endPoint.rawX;
            totalY += endPoint.rawY;
            count++;

            return new DataPoint(totalX / count, totalY / count);
        }
    }

    public struct DataPoint
    {
        public double rawX;
        public double rawY;
        public double timeStamp;

        public DataPoint(double X, double Y)
        {
            this.rawX = X;
            this.rawY = Y;
            this.timeStamp = -1;
        }

        public DataPoint(double X, double Y, double timeStamp)
        {
            this.rawX = X;
            this.rawY = Y;
            this.timeStamp = timeStamp;
        }

        public Point toPoint()
        {
            return new Point(this.rawX, this.rawY);
        }

        public Point toClientPoint()
        {
            return new Point(0,0);
        }

        public String toString()
        {
            return "{" + rawX + "," + rawY + "}";
        }
    }

    public struct Saccade
    {
        public double X1;
        public double X2;
        public double Y1;
        public double Y2;
        public double start;
        public double end;
        public double hlength;
        public double vlength;
        public double size;
        public double duration;

        //public Boolean isRegression;

        //public Saccade(double X1, double Y1, double X2, double Y2, double timestamp)
        //{
        //    this.X1 = X1;
        //    this.X2 = X2;
        //    this.Y1 = Y1;
        //    this.Y2 = Y2;
        //    this.timeStamp = timestamp;
        //    hlength = X2 - X1;
        //    vlength = Y2 - Y1;
        //    length = Math.Sqrt(Math.Pow(hlength, 2) + Math.Pow(vlength, 2));
        //    //isRegression = (vlength < 0);
        //}


        //public Saccade(Point p1, Point p2, double timestamp)
        //{
        //    X1 = p1.X;
        //    X2 = p2.X;
        //    Y1 = p1.Y;
        //    Y2 = p2.Y;
        //    this.start = timestamp;
        //    hlength = X2 - X1;
        //    vlength = Y2 - Y1;
        //    length = Math.Sqrt(Math.Pow(hlength, 2) + Math.Pow(vlength, 2));
        //    //isRegression = (vlength < 0);
        //}
    }

    
}



