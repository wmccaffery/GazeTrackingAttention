
using Microsoft.Expression.Encoder.Devices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Tobii.Interaction;
using UserControl = System.Windows.Controls.UserControl;

namespace GazeTrackingAttentionDemo
{


	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		enum State {Wait, Setup, Test, ReadyToRecord, Recording, DoneRecording, Markup}
		//Wait: time between tests
		//Setup: user creation and initial calibration
		//Test: testing user calibration
		//ReadyToRecord: ready to start recording data
		//Recording: data is being recorded
		//DoneRecording: data has been recorded
		//Markup: data is being annotated

		Boolean showGaze;
		Boolean showFixations;
		Boolean showSaccades;
		Boolean dataRecorded;

		State state  = State.ReadyToRecord; //TODO INITIAL VALUE IS CURRENTLY SET FOR DEBUGGING

		UserControl document = new UserControls.DocumentCtrl();
		UserControl test = new UserControls.TestCalibrationCtrl();
		UserControl markup = new UserControls.MarkupCtrl();


		GazeStreamReader fd;
		Session session;
		TestSet testSet;

		public Collection<EncoderDevice> VideoDevices { get; set; }
		public Collection<EncoderDevice> AudioDevices { get; set; }


		//double displayTime;

		public MainWindow()
		{
			//set window to start on the second monitor
			this.WindowStartupLocation = WindowStartupLocation.Manual;
			Screen s2 = Screen.AllScreens[0]; //Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();
			System.Drawing.Rectangle r2 = s2.WorkingArea;
			this.Top = r2.Top;
			this.Left = r2.Left;
			this.Width = r2.Width;
			this.Height = r2.Height;

			this.DataContext = this;

			InitializeComponent();

			//find available audio and video devices for webcam
			VideoDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);
			AudioDevices = EncoderDevices.FindDevices(EncoderDeviceType.Audio);

			init();

			//Create child window
			//SourceInitialized += (s, a) =>
			//{
			//	ControlWindow ctrlwin = new ControlWindow();
			//	ctrlwin.Owner = this;
			//	ctrlwin.Show();
			//};

			centerView.Content = document;
		}

		public void init()
		{
			session = new Session();
			fd = new GazeStreamReader(session);
			//Render.IsEnabled = false;
			//Begin.IsEnabled = true;
			//End.IsEnabled = false;
			//Test_Callibration.IsEnabled = true;
			//Callibrate.IsEnabled = true;
			//Reset.IsEnabled = true;
			//mainCanvas.Children.Clear();


			//MarkupSubGrid.Visibility = Visibility.Hidden;
			//DocumentArea.Visibility = Visibility.Visible;
			//DisplaySlider.Visibility = Visibility.Hidden;
			//DisplaySlider_Value.Visibility = Visibility.Hidden;
			//All_CheckBox.Visibility = Visibility.Hidden;
			//Saccade_CheckBox.Visibility = Visibility.Hidden;
			//Fixation_CheckBox.Visibility = Visibility.Hidden;
			//Gaze_CheckBox.Visibility = Visibility.Hidden;
			//Visualisation_Header.Visibility = Visibility.Hidden;


		}

		public void onLoad(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Maximized;
			this.KeyDown += new System.Windows.Input.KeyEventHandler(MainWindow_KeyDown);

		}

		//private void Exit_Click(object sender, RoutedEventArgs e)
		//{
		//	System.Windows.Application.Current.Shutdown();
		//}

		//public void Test_Click(object sender, RoutedEventArgs e)
		//{
		//	if (test)
		//	{
		//		test = false;
		//		init();
		//	}
		//	else
		//	{
		//		test = true;
		//	}
		//render();

		//PageText.Visibility = Visibility.Hidden;

		//}

		//private void Begin_Click(object sender, RoutedEventArgs e)
		//{
		//	//fd = new DataReader();
		//	fd.readFixationStream();
		//	fd.readGazeStream();
		//	//End.IsEnabled = true;
		//	//Begin.Visibility = Visibility.Collapsed;
		//}

		private void End_Click(object sender, RoutedEventArgs e)
		{
			fd.Dispose();
			//Render.IsEnabled = true;
			//Begin.IsEnabled = false;
			//End.IsEnabled = false;
			//Test_Callibration.IsEnabled = false;
			//Callibrate.IsEnabled = false;
			DisplaySlider.Maximum = session.currentTestResults.endTime;
			DisplaySlider.Minimum = session.currentTestResults.startTime;
			dataRecorded = true;


		}

		private void Callibrate_Click(object sender, RoutedEventArgs e)
		{
			fd.callibrate();
		}

		private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			//if (e.Key == Key.A)
			//{
			//	Environment.Exit(0);
			//}
			switch (e.Key)
			{
				case Key.R:
					//Reset
					init();
					break;
				case Key.E:
					//Exit
					Environment.Exit(0);
					break;
				case Key.N:
					//Next
					//load next doc
					break;
				case Key.S:
					//Start/Stop recording data
					if (state == State.ReadyToRecord)
					{
						fd.readFixationStream();
						fd.readGazeStream();
						state = State.Recording;
					}
					else if (state == State.Recording)
					{
						fd.Dispose();
						state = State.DoneRecording;
					}
					break;
				case Key.C:
					//calibrate
					break;
				case Key.T:
					//test calibration
					if (state == State.ReadyToRecord)
					{
						centerView.Content = test;
						state = State.Test;
					}
					else if (state == State.Test)
					{
						centerView.Content = document;
						state = State.ReadyToRecord;
					}
					break;
			}
		}

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            init();
        }

		private void btnPlay_Click(object sender, RoutedEventArgs e)
		{

			player.Play();
		}

		private void btnPause_Click(object sender, RoutedEventArgs e)
		{
			player.Pause();
		}

		private void btnStop_Click(object sender, RoutedEventArgs e)
		{
			player.Stop();
		}

		private void Render_Click(object sender, RoutedEventArgs e)
        {
            //DisplaySlider.Value = DisplaySlider.Maximum;
            DisplaySlider.Visibility = Visibility.Visible;
            DisplaySlider_Value.Visibility = Visibility.Visible;
            All_CheckBox.Visibility = Visibility.Visible;
            Saccade_CheckBox.Visibility = Visibility.Visible;
            Fixation_CheckBox.Visibility = Visibility.Visible;
            Gaze_CheckBox.Visibility = Visibility.Visible;
            //Visualisation_Header.Visibility = Visibility.Visible;
            //render();
        }

        private void DisplayTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //displayTime = DisplaySlider.Value;
            //render();
        }

        private void Gaze_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            showGaze = true;
        }

        private void Gaze_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            showGaze = false;
        }

        private void Fixation_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            showFixations = true;
        }

        private void Fixation_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            showFixations = false;
        }

        private void Saccade_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            showSaccades = true;
        }

        private void Saccade_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            showSaccades = false;
        }

        private void All_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Saccade_CheckBox.IsChecked = true;
            Fixation_CheckBox.IsChecked = true;
            Gaze_CheckBox.IsChecked = true;
        }

        private void All_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Saccade_CheckBox.IsChecked = false;
            Fixation_CheckBox.IsChecked = false;
            Gaze_CheckBox.IsChecked = false;
        }

		private void Load_Click(object sender, RoutedEventArgs e)
		{
			//    //select document to load 

			//    //load paragraphs into document text area

			//    //make use of rtb load function?
			//    p1text.Text = "THIS IS PARAGRAPH 1";
			//    p2text.Text = "THIS IS PARAGRAPH 2";
			//    p3text.Text = "THIS IS PARAGRAPH 3";
			//    p4text.Text = "THIS IS PARAGRAPH 4";


		}

		//private void RtbMouseMove(object sender, MouseEventArgs e)
		//{
		//    RichTextBox rtb = sender as RichTextBox;
		//    rtb.Focus();
		//    Point point = Mouse.GetPosition(rtb);

		//    TextPointer tp = rtb.GetPositionFromPoint(point, true);
		//    Paragraph p = tp.Paragraph;

		//    TextPointer start = p.ContentStart;
		//    TextPointer end = p.ContentEnd;

		//    rtb.Selection.Select(start, end);
		//}

		//private void RtbMouseDown(object sender, MouseButtonEventArgs e)
		//{
		//    Console.WriteLine("Triggered");
		//    RichTextBox rtb = sender as RichTextBox;
		//    TextPointer start = rtb.Selection.Start;
		//    Paragraph p = start.Paragraph;
		//    Console.WriteLine("paragraph " + p.Name + " selected");
		//}

		//private void render()
		//{
		//    StackTrace stackTrace = new StackTrace();
		//    Console.WriteLine("I was called by " + stackTrace.GetFrame(1).GetMethod().Name);
		//    mainCanvas.Children.Clear();
		//    //if (State.fixations.Count == 0)
		//    //{
		//    //    Console.WriteLine("nothing to render");
		//    //}
		//        if (test)
		//        {
		//            renderTestTargets();
		//        }

		//        if (showFixations)
		//        {
		//            //double durationSum = 0;
		//            for (int i = 0; i < session.currentTestResults.fixationData.Count; i++)
		//            {
		//                Fixation f = (Fixation)session.currentTestResults.fixationData[i];
		//                //if (f.startPoint.timeStamp <= displayTime)
		//                {
		//                Console.WriteLine("duration " + f.duration);
		//                renderEllipse(this.CenterSubGrid.PointFromScreen(f.centroid.toPoint()), Color.FromArgb(127, Colors.Blue.R, Colors.Blue.G, Colors.Blue.B), i,Math.Sqrt(f.duration/100/Math.PI));
		//                }
		//            }
		//        }

		//        if (showSaccades)
		//        {
		//            foreach (Saccade l in session.currentTestResults.SaccadeData)
		//            {
		//                {
		//                    //if (l.start <= displayTime)
		//                    renderLine(l, Colors.Black);
		//                }
		//            }
		//        }

		//        if (showGaze)
		//        {
		//            foreach (DataPoint p in session.currentTestResults.rawGazeData)
		//            {
		//            //if (p.timeStamp <= displayTime)
		//            //{
		//                    //renderEllipse(p.toPoint(), Colors.Red, 5);
		//                    Point pt = this.CenterSubGrid.PointFromScreen(p.toPoint());
		//                    renderEllipse(pt, Colors.Gold, 5);
		//                //}
		//            }
		//        }
		//    }


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

		//private void renderEllipse(Point p, Color c, double radius)
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
		//}

		//private void renderLine(Saccade line, Color c)
		//{
		//    try
		//    {
		//        Point p1 = new Point();
		//        Point p2 = new Point();

		//        p1.X = line.X1;
		//        p1.Y = line.Y1;
		//        p2.X = line.X2;
		//        p2.Y = line.Y2;

		//        Point p1_fixed = this.CenterSubGrid.PointFromScreen(p1);
		//        Point p2_fixed = this.CenterSubGrid.PointFromScreen(p2);

		//        Line l = new Line();
		//        l.X1 = p1_fixed.X;
		//        l.X2 = p2_fixed.X;
		//        l.Y1 = p1_fixed.Y;
		//        l.Y2 = p2_fixed.Y;
		//        l.Stroke = new SolidColorBrush(c);
		//        l.StrokeThickness = 2;
		//        mainCanvas.Children.Add(l);
		//    }
		//    catch (Exception e)
		//    {
		//        //the bug that caused this should have been patched out
		//        Console.WriteLine("WARN: line failed to render");
		//    }
		//}

		private void CheckBox_Clicked(object sender, RoutedEventArgs e)
		{
			//    render();
		}
}

    //Handle data streams from the eye tracker
    public class GazeStreamReader : IDisposable
    {
        private readonly Host _host;
        private readonly FixationDataStream _fixationDataStream;
        private readonly GazePointDataStream _gazePointDataStream;
        private Session session;
        

        public GazeStreamReader(Session session)
        {
            _host = new Host();
            _fixationDataStream = _host.Streams.CreateFixationDataStream();
            //_fixationDataStream = _host.Streams.CreateFixationDataStream(Tobii.Interaction.Framework.FixationDataMode.Slow);
            _gazePointDataStream = _host.Streams.CreateGazePointDataStream();
            this.session = session;
        }

        public void callibrate()
        {
            _host.Context.LaunchConfigurationTool(Tobii.Interaction.Framework.ConfigurationTool.Recalibrate, (data) => { });
        }

        public void readFixationStream()
        {
            //_fixationDataStream
            //    .Begin((x, y, _) =>
            //    {
            //        Console.WriteLine("\n" +
            //                          "Fixation started at X: {0}, Y: {1}", x, y);
            //        _fixationBeginTime = DateTime.Now;
            //    })
            //    .Data((x, y, _) =>
            //    {
            //        Console.WriteLine("During fixation, currently at: X: {0}, Y: {1}", x, y);
            //    })
            //    .End((x, y, _) =>
            //    {
            //        Console.WriteLine("Fixation ended at X: {0}, Y: {1}", x, y);
            //        if (_fixationBeginTime != default(DateTime))
            //        {
            //            Console.ForegroundColor = ConsoleColor.Cyan;
            //            Console.WriteLine("Fixation duration: {0}", DateTime.Now - _fixationBeginTime);
            //            Console.ForegroundColor = _defaultForegroundColor;
            //        }
            //    });

            //store fixation
            Fixation f = new Fixation();
            long startTime = -1;
            long endTime = -1;

            _fixationDataStream
                .Begin((x, y, timestamp) =>
            {
                
                f.startPoint = new DataPoint(x, y, timestamp);
                Console.WriteLine("Fixation started at X:{0} Y:{1} Device timestamp: {2}", x, y, timestamp);
                startTime = Stopwatch.GetTimestamp();
            })
                 .Data((x, y, timestamp) =>
            {
                f.points.Add(new DataPoint(x, y, timestamp));
                Console.WriteLine("During fixation at X:{0} Y:{1} Device timestamp: {2}", x, y, timestamp);

            })
                .End((x, y, timestamp) =>
            {
                endTime = Stopwatch.GetTimestamp();
                f.endPoint = new DataPoint(x, y, timestamp);
                if ((Double.IsNaN(f.endPoint.rawX) && Double.IsNaN(f.endPoint.rawY)) || startTime == -1) 
                {
                    Console.WriteLine("NOT A VALID FIXATION, DISCARDED");
                }
                else
                {
                    Console.WriteLine("Fixation started at " + f.startPoint.timeStamp);
                    Console.WriteLine("Fixation finished at X:{0} Y:{1} Device timestamp: {2}", x, y, timestamp);
                    session.currentTestResults.fixationData.Add(f);
                }
                f.duration = endTime - startTime;
                f.completeFixation();
                findSaccade();
                f = new Fixation();
            });

            //f.duration = f.endPoint.timeStamp - f.startPoint.timeStamp;

            //if (!Double.IsNaN(f.startPoint.rawX) && !Double.IsNaN(f.startPoint.rawY) && !Double.IsNaN(f.endPoint.rawX) && !Double.IsNaN(f.endPoint.rawY))
            //{
            //    f.completeFixation();
            //    //Console.WriteLine(f.startPoint.X);
            //    //Console.WriteLine("duration {0})", f.duration);
            //    if (f.startPoint.timeStamp != 0)
            //    {
            //        session.currentTestResults.fixationData.Add(f);
            //        Console.WriteLine("NEW FIXATION: \n" +
            //        "xPos:{0}\n" +
            //        "yPos:{1}\n" +
            //        "timestamp:{2}\n" +
            //        "duration:{3}\n" +
            //        "startPoint:{4}\n" +
            //        "subPoints:{5}\n" +
            //        "endPoint:{6}\n", f.centroid.rawX, f.centroid.rawY, f.startPoint.timeStamp, f.duration, f.startPoint.toString(), f.points.ToString(), f.endPoint.toString());
            //        //attemptSaccade();
            //        f = new Fixation();
            //    }
            //}

            //f.getLines();

            //});
        }

        public void readGazeStream()
        {
            _gazePointDataStream.GazePoint((x, y, timestamp) =>
            {
                session.currentTestResults.rawGazeData.Add(new DataPoint(x, y, timestamp));
                //Console.WriteLine("NEW GAZE POINT: \n xPos: " + x + "\n yPos: " + y + "\n timestamp " + timestamp);
                if(session.currentTestResults.startTime == -1)
                {
                    session.currentTestResults.startTime = timestamp; 
                }
                if(timestamp > session.currentTestResults.endTime)
                {
                    session.currentTestResults.endTime = timestamp;
                }
            });
        }

        private void findSaccade()
        {
            //store saccade
            
            if (session.currentTestResults.fixationData.Count > 1)
            {
                Saccade s = new Saccade();
                Fixation f1 = ((Fixation)session.currentTestResults.fixationData[session.currentTestResults.fixationData.Count - 2]);
                Fixation f2 = ((Fixation)session.currentTestResults.fixationData[session.currentTestResults.fixationData.Count - 1]);
                s.X1 = f1.centroid.rawX;
                s.Y1 = f1.centroid.rawY;
                s.X2 = f2.centroid.rawX;
                s.Y2 = f2.centroid.rawY;
                s.hlength = s.X2 - s.X1;
                s.vlength = s.Y2 - s.Y1;
                s.size = Math.Sqrt(Math.Pow(s.hlength, 2) + Math.Pow(s.vlength, 2));
                //s.start = f1.endPoint.timeStamp;
                //s.end = f2.startPoint.timeStamp;
                //s.duration = f2.startPoint.timeStamp - f1.endPoint.timeStamp;
                session.currentTestResults.SaccadeData.Add(s);

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

    //
    //public static class DataManager
    //{
    //    public static Mode mode;

    //    //TODO implement navigation for set -- something like the word screen maybe? probably too ornate.
    //    public static ArrayList testSet = new ArrayList();

    //    public static Session currentTest = new Session();
        


        //public static double getLastTimestamp()
        //{
        //    return ((Fixation)fixationData[fixationData.Count - 1]).endPoint.timeStamp;
        //}

        //public static double getFirstTimestamp()
        //{
        //    return ((Fixation)fixationData[0]).startPoint.timeStamp;
        //}
    //}

    public class Session
    {
        public Session()
        {
            TestResults = new List<ResultsSet>();
            currentTestResults = new ResultsSet();
        }

        public String userID;
        public ResultsSet currentTestResults;
        public List<ResultsSet> TestResults;
    }

    public class ResultsSet
    {
        public ResultsSet()
        {
            fixationData = new List<Fixation>();
            rawGazeData = new List<DataPoint>();
            SaccadeData = new List<Saccade>();

            startTime = -1;
            endTime = -1;
        }

        public List<Fixation> fixationData;
        public List<DataPoint> rawGazeData;
        public List<Saccade> SaccadeData;

        public double startTime;
        public double endTime;
    }

    public class TestSet
    {
        public List<String> testPaths;
        int index;

        public String getCurrent()
        {
            return testPaths[index];
        }

        public int nextTest()
        {
            if (index < testPaths.Count - 1)
            {
                index++;
                return 0;
            }
            return 1;
        }

        public int prevTest()
        {
            if (index > 0)
            {
                index--;
                return 0;
            }
            return 1;
        }

        //public void loadTestSet()
        //{
            
        //}
    }

    public class AnnotatedParagraph
    {
        String text;
        int attention;
        int effort;
        String comments;
    }

    public class Fixation
    {
        //Point point1;
        //Point point2;

        public DataPoint startPoint;
        public ArrayList points;
        public DataPoint endPoint;
        //public ArrayList lines;
        public DataPoint centroid;

        public double duration;

        //move draw function inside here
        public Fixation()
        {
            points = new ArrayList();
            //lines = new ArrayList();
        }

        public void completeFixation()
        {
            //if (points.Count > 0)
            //{
            //TODO implement time weights

            ////calculate average duration of points (apart from end point)
            //startPoint.duration = ((DataPoint)points[0]).timeStamp - startPoint.timeStamp;
            //double pointDurationSum = startPoint.duration;
            //for (int i = 1; i < points.Count; i++)
            //{
            //    DataPoint p = ((DataPoint)points[i]);
            //    DataPoint prev = ((DataPoint)points[i - 1]);
            //    p.duration = p.timeStamp - prev.timeStamp;
            //    pointDurationSum += p.duration;
            //}
            //double avgPointDuration = pointDurationSum / (points.Count + 1);

            ////calculate percentage difference from average for each point (apart from end point)
            //startPoint.weight = (startPoint.duration - avgPointDuration) / 100;

            //for (int i = 0; i < points.Count; i++)
            //{
            //    DataPoint p = ((DataPoint)points[i]);
            //    p.weight += (p.duration - avgPointDuration) / 100;
            //}

            //get average x and y coordinates


            double xsum = 0;
            double ysum = 0;
            int numPoints = 0;

            xsum += startPoint.rawX;
            ysum += startPoint.rawY;
            numPoints++;
            

            for (int i = 0; i < points.Count; i++)
            {
                DataPoint p = ((DataPoint)points[i]);
                xsum += p.rawX;
                ysum += p.rawY;
                numPoints++;
            }

            xsum += endPoint.rawX;
            ysum += endPoint.rawY;
            numPoints++;
            

            centroid = new DataPoint((xsum / numPoints), (ysum / numPoints));

                //double cx = xsum / points.Count + 2;
                //double cy = ysum / points.Count + 2;
                //centroid = new DataPoint(cx, cy);W
            }
        //}

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
        //public DataPoint getCentroid()
        //{
        //    double totalX = 0;
        //    double totalY = 0;
        //    int count = 0;

        //    totalX += startPoint.rawX;
        //    totalY += startPoint.rawY;
        //    count++;

        //    foreach (DataPoint p in points)
        //    {
        //        totalX += p.rawX;
        //        totalY += p.rawY;
        //        count++;
        //    }

        //    totalX += endPoint.rawX;
        //    totalY += endPoint.rawY;
        //    count++;

        //    return new DataPoint(totalX / count, totalY / count);
        //}
    }

    public struct DataPoint
    {
        public double rawX;
        public double rawY;
        public double timeStamp;
        public double duration;
        public double weight;

        public DataPoint(double X, double Y)
        {
            this.rawX = X;
            this.rawY = Y;
            this.timeStamp = -1;
            this.duration = -1;
            this.weight = -1;
        }

        public DataPoint(double X, double Y, double timeStamp)
        {
            this.rawX = X;
            this.rawY = Y;
            this.timeStamp = timeStamp;
            this.duration = -1;
            this.weight = -1;
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



