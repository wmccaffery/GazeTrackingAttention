
using GazeTrackingAttentionDemo.LSLInteraction;
using GazeTrackingAttentionDemo.Models;
using LSL;
using Microsoft.Expression.Encoder.Devices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Tobii.Interaction;
using Tobii.Interaction.Framework;
using UserControl = System.Windows.Controls.UserControl;

namespace GazeTrackingAttentionDemo
{


	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public enum EState { Wait, Setup, Test, ReadyToCalibrate, Ready, Streaming, Recording, DoneRecording, Markup }
		//Wait: time between tests
		//Setup: user creation and initial calibration
		//Test: testing user calibration
		//ReadyToRecord: ready to start recording data
		//Recording: data is being recorded
		//DoneRecording: data has been recorded
		//Markup: data is being annotated


		public EState State { get; set; }
		//{
		//get { return State; }
		//set
		//{
		//	State = value;
		//	if (State == EState.Markup)
		//	{
		//		//show markup
		//	}
		//}
		//}


		UserControl document = new UserControls.DocumentCtrl();
		UserControl test = new UserControls.TestCalibrationCtrl();
		UserControl markup = new UserControls.MarkupCtrl();

		//current user data
		public User currentUser;
		public Test currentTestInstance;

		//list of all file paths
		public List<String> filePaths { get; set; }

		//index of next filePath to read
		int testIndex = 0;

		//handler and event for loading tests
		public delegate void LoadTestHandler(String test);
		public event LoadTestHandler loadTest;

		//init gaze stream
		//StreamReader fd;

		//Thread eegStream;

		//Session session;

		//collection of all video devices available
		public Collection<EncoderDevice> VideoDevices { get; set; }
		//public Collection<EncoderDevice> AudioDevices { get; set; }

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

			//find available video devices for webcam
			VideoDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);

			loadTest += new LoadTestHandler(((UserControls.DocumentCtrl)document).onTestLoaded);

			//Create child window
			SourceInitialized += (s, a) =>
			{
				ControlWindow ctrlwin = new ControlWindow();
				ctrlwin.Owner = this;
				ctrlwin.Show();
			};
		}


		public void onLoad(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Maximized;
			this.KeyDown += new System.Windows.Input.KeyEventHandler(MainWindow_KeyDown);

		}

		public void onUserCreated(User user)
		{
			//fd.calibrate();
			centerView.Content = document;

			currentUser = user;
			State = EState.ReadyToCalibrate;

			//load test files
			filePaths = new List<String>(Directory.GetFiles(currentUser.GroupPath));
			filePaths.Sort(delegate (String a, String b)
			{
				int a_num = Int32.Parse(Regex.Match(a, @"\d+").Value);
				int b_num = Int32.Parse(Regex.Match(a, @"\d+").Value);
				return a.CompareTo(b);
			});
			testIndex = -1;
		}

		//loads current testindex returns false if testindex too high
		//public getTest()
		//{
		//	if (testIndex < filePaths.Count)
		//	{
		//		loadTest(filePaths[testIndex]);
		//		testIndex++;
		//		return true;
		//	}
		//}

		public Boolean incrementTestCounter()
		{
			if (testIndex < filePaths.Count)
			{
				testIndex++;
				return true;
			}
			return false;
		}

		public String getCurrentTest()
		{
			return filePaths[testIndex];
		}
		

		public void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.R:
					//Restart current test
					break;
				case Key.E:
					//exit program
					Environment.Exit(0);
					break;
				case Key.N:
					//load next test, when finished start markup
					if(State == EState.ReadyToCalibrate || State == EState.DoneRecording)
					{
						if (!incrementTestCounter())
						{
							State = EState.Markup;
						} else
						{
							Console.WriteLine("Starting test " + testIndex);
							currentTestInstance = new Test(currentUser, filePaths[testIndex], testIndex);
							currentTestInstance.fd.calibrate();
							State = EState.Ready;
						}
					}
					break;
				case Key.S:
					//Start/Stop recording data
					if (State == EState.Ready)
					{
						loadTest(filePaths[testIndex]);
						Console.WriteLine("Loading test " + filePaths[testIndex]);

						currentTestInstance.fd.feedStreamsToLSL();
						currentTestInstance.fd.readStreams();

						Console.WriteLine("Started Reading streams to LSL");

						State = EState.Streaming;
					} else if (State == EState.Streaming)
					{
						currentTestInstance.fd.recordStreams();
						State = EState.Recording;
					} else if (State == EState.Recording)
					{
						currentTestInstance.fd.stopRecording();
						Console.WriteLine("Test " + testIndex + " complete");
						currentTestInstance.testComplete();
						currentTestInstance.fd.closeLslStreams();
						State = EState.DoneRecording;
					}
					break;
			}
		}

		//public void onDeviceCalibration(object sender, EngineStateValue<EyeTrackingDeviceStatus> e)
		//{
		//	EngineStateObserver<EyeTrackingDeviceStatus> _sender = (EngineStateObserver<EyeTrackingDeviceStatus>)sender;
		//	if (!_sender.CurrentValue.Equals(EyeTrackingDeviceStatus.Configuring)) { 
		//		Console.WriteLine("CALIBRATED " + sender.GetType());
		//		loadTest(filePaths[testIndex]);
		//		State = EState.Ready;
		//	}
		//}


		private void DisplayTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //displayTime = DisplaySlider.Value;
            //render();
        }

		
}

	//Handle data streams from the eye tracker
	

    //public class Session
    //{
    //    public Session()
    //    {
    //        TestResults = new List<ResultsSet>();
    //        currentTestResults = new ResultsSet();
    //    }

    //    public String userID;
    //    public ResultsSet currentTestResults;
    //    public List<ResultsSet> TestResults;
    //}

    //public class ResultsSet
    //{
    //    public ResultsSet()
    //    {
    //        fixationData = new List<Fixation>();
    //        rawGazeData = new List<DataPoint>();
    //        SaccadeData = new List<Saccade>();

    //        startTime = -1;
    //        endTime = -1;
    //    }

    //    public List<Fixation> fixationData;
    //    public List<DataPoint> rawGazeData;
    //    public List<Saccade> SaccadeData;

    //    public double startTime;
    //    public double endTime;
    //}

    //public class Fixation
    //{
    //    //Point point1;
    //    //Point point2;

    //    public DataPoint startPoint;
    //    public ArrayList points;
    //    public DataPoint endPoint;
    //    //public ArrayList lines;
    //    public DataPoint centroid;

    //    public double duration;

    //    //move draw function inside here
    //    //public Fixation()
    //    //{
    //    //    points = new ArrayList();
    //    //    //lines = new ArrayList();
    //    //}

    //    //public void completeFixation()
    //    //{
    //    //    //if (points.Count > 0)
    //    //    //{
    //    //    //TODO implement time weights

    //    //    ////calculate average duration of points (apart from end point)
    //    //    //startPoint.duration = ((DataPoint)points[0]).timeStamp - startPoint.timeStamp;
    //    //    //double pointDurationSum = startPoint.duration;
    //    //    //for (int i = 1; i < points.Count; i++)
    //    //    //{
    //    //    //    DataPoint p = ((DataPoint)points[i]);
    //    //    //    DataPoint prev = ((DataPoint)points[i - 1]);
    //    //    //    p.duration = p.timeStamp - prev.timeStamp;
    //    //    //    pointDurationSum += p.duration;
    //    //    //}
    //    //    //double avgPointDuration = pointDurationSum / (points.Count + 1);

    //    //    ////calculate percentage difference from average for each point (apart from end point)
    //    //    //startPoint.weight = (startPoint.duration - avgPointDuration) / 100;

    //    //    //for (int i = 0; i < points.Count; i++)
    //    //    //{
    //    //    //    DataPoint p = ((DataPoint)points[i]);
    //    //    //    p.weight += (p.duration - avgPointDuration) / 100;
    //    //    //}

    //    //    //get average x and y coordinates


    //    //    double xsum = 0;
    //    //    double ysum = 0;
    //    //    int numPoints = 0;

    //    //    xsum += startPoint.rawX;
    //    //    ysum += startPoint.rawY;
    //    //    numPoints++;
            

    //    //    for (int i = 0; i < points.Count; i++)
    //    //    {
    //    //        DataPoint p = ((DataPoint)points[i]);
    //    //        xsum += p.rawX;
    //    //        ysum += p.rawY;
    //    //        numPoints++;
    //    //    }

    //    //    xsum += endPoint.rawX;
    //    //    ysum += endPoint.rawY;
    //    //    numPoints++;
            

    //    //    centroid = new DataPoint((xsum / numPoints), (ysum / numPoints));

    //    //        //double cx = xsum / points.Count + 2;
    //    //        //double cy = ysum / points.Count + 2;
    //    //        //centroid = new DataPoint(cx, cy);W
    //    //    }
    //    ////}

    //    //public void getLines()
    //    //{
    //    //check for existing fixations
    //    //if (State.fixations.Count > 1)
    //    //{
    //    //    point1 = ((Fixation)State.fixations[State.fixations.Count-2]).endPoint;

    //    //    point2 = startPoint;
    //    //    lines.Add(new LineWithTimestamp(point1, point2, point2.timeStamp));
    //    //}

    //    //point1 = startPoint;

    //    //foreach(Point p in points)
    //    //{
    //    //    point2 = p;
    //    //    lines.Add(new LineWithTimestamp(point1, point2, point2.timeStamp));

    //    //    point1 = p;
    //    //}

    //    //point2 = endPoint;
    //    //lines.Add(new LineWithTimestamp(point1, point2, point2.timeStamp));
    //    //}


    //    //TODO add weighting using timestamp
    //    //public DataPoint getCentroid()
    //    //{
    //    //    double totalX = 0;
    //    //    double totalY = 0;
    //    //    int count = 0;

    //    //    totalX += startPoint.rawX;
    //    //    totalY += startPoint.rawY;
    //    //    count++;

    //    //    foreach (DataPoint p in points)
    //    //    {
    //    //        totalX += p.rawX;
    //    //        totalY += p.rawY;
    //    //        count++;
    //    //    }

    //    //    totalX += endPoint.rawX;
    //    //    totalY += endPoint.rawY;
    //    //    count++;

    //    //    return new DataPoint(totalX / count, totalY / count);
    //    //}
    //}

    //public struct DataPoint
    //{
    //    public double rawX;
    //    public double rawY;
    //    public double timeStamp;
    //    public double duration;

    //    public DataPoint(double X, double Y)
    //    {
    //        this.rawX = X;
    //        this.rawY = Y;
    //        this.timeStamp = -1;
    //        this.duration = -1;
    //    }

    //    public DataPoint(double X, double Y, double timeStamp)
    //    {
    //        this.rawX = X;
    //        this.rawY = Y;
    //        this.timeStamp = timeStamp;
    //        this.duration = -1;
    //    }

    //    public Point toPoint()
    //    {
    //        return new Point(this.rawX, this.rawY);
    //    }

    //    public Point toClientPoint()
    //    {
    //        return new Point(0,0);
    //    }

    //    public String toString()
    //    {
    //        return "{" + rawX + "," + rawY + "}";
    //    }
    //}

    //public struct Saccade
    //{
    //    public double X1;
    //    public double X2;
    //    public double Y1;
    //    public double Y2;
    //    public double start;
    //    public double end;
    //    public double hlength;
    //    public double vlength;
    //    public double size;
    //    public double duration;

    //    //public Boolean isRegression;

    //    //public Saccade(double X1, double Y1, double X2, double Y2, double timestamp)
    //    //{
    //    //    this.X1 = X1;
    //    //    this.X2 = X2;
    //    //    this.Y1 = Y1;
    //    //    this.Y2 = Y2;
    //    //    this.timeStamp = timestamp;
    //    //    hlength = X2 - X1;
    //    //    vlength = Y2 - Y1;
    //    //    length = Math.Sqrt(Math.Pow(hlength, 2) + Math.Pow(vlength, 2));
    //    //    //isRegression = (vlength < 0);
    //    //}


    //    //public Saccade(Point p1, Point p2, double timestamp)
    //    //{
    //    //    X1 = p1.X;
    //    //    X2 = p2.X;
    //    //    Y1 = p1.Y;
    //    //    Y2 = p2.Y;
    //    //    this.start = timestamp;
    //    //    hlength = X2 - X1;
    //    //    vlength = Y2 - Y1;
    //    //    length = Math.Sqrt(Math.Pow(hlength, 2) + Math.Pow(vlength, 2));
    //    //    //isRegression = (vlength < 0);
    //    //}
    //}

    
}



