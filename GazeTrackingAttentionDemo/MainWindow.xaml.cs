
using GazeTrackingAttentionDemo.Models;
using LSL;
using Microsoft.Expression.Encoder.Devices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Tobii.Interaction;
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
		User currentUser;

		//list of all file paths
		public List<String> filePaths { get; set; }

		//index of next filePath to read
		int testIndex = 0;

		//handler and event for loading tests
		public delegate void LoadTestHandler(String test);
		public event LoadTestHandler loadTest;

		//init gaze stream
		GazeStreamReader fd;
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


			//session = new Session();
			fd = new GazeStreamReader();

			init();

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
			//centerView.Content = test;
			
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
			testIndex = 0;
		}

		public void init()
		{
			//session = new Session();
			//fd = new GazeStreamReader(session);

		}

		//loads current testindex returns false if testindex too high
		public Boolean getTest()
		{
			if (testIndex < filePaths.Count)
			{
				loadTest(filePaths[testIndex]);
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
						if (!getTest())
						{
							State = EState.Markup;
						} else
						{
							fd.calibrate();
							State = EState.Ready;
						}
					}
					break;
				case Key.S:
					//Start/Stop recording data
					if(State == EState.Ready)
					{
						fd.readStreams();
					}
					if (State == EState.Streaming)
					{
						fd.recordStreams();
						State = EState.Recording;
					} else if (State == EState.Recording)
					{
						fd.Dispose();
						State = EState.DoneRecording;
					}
					break;
			}
		}


        private void DisplayTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //displayTime = DisplaySlider.Value;
            //render();
        }

		
}

    //Handle data streams from the eye tracker
    public class GazeStreamReader : IDisposable
    {
        private readonly Host _host;
        private readonly FixationDataStream _fixationDataStream;
        private readonly GazePointDataStream _gazePointDataStream;
		//private Session session;

		//LSL output streams
		public liblsl.StreamInfo gazeDataInfo;
		public liblsl.StreamInfo[] gazeDataResultsInfo;
		public liblsl.StreamOutlet gazeDataOutlet;
		public liblsl.StreamInlet gazeDataInlet;

		public liblsl.StreamInfo fixationBeginInfo;
		public liblsl.StreamInfo[] fixationBeginResultsInfo;
		public liblsl.StreamOutlet fixationBeginOutlet;
		public liblsl.StreamInlet fixationBeginInlet;

		public liblsl.StreamInfo fixationDataInfo;
		public liblsl.StreamInfo[] fixationDataResultsInfo;
		public liblsl.StreamOutlet fixationDataOutlet;
		public liblsl.StreamInlet fixationDataInlet;

		public liblsl.StreamInfo fixationEndInfo;
		public liblsl.StreamInfo[] fixationEndResultsInfo;
		public liblsl.StreamOutlet fixationEndOutlet;
		public liblsl.StreamInlet fixationEndInlet;

		public liblsl.StreamInfo[] eegDataResultsInfo;
		public liblsl.StreamInlet eegDataInlet;


		public GazeStreamReader()
        {
            _host = new Host();
            _fixationDataStream = _host.Streams.CreateFixationDataStream();
            //_fixationDataStream = _host.Streams.CreateFixationDataStream(Tobii.Interaction.Framework.FixationDataMode.Slow);
            _gazePointDataStream = _host.Streams.CreateGazePointDataStream();
            //this.session = session;
        }

		public void initLSL()
		{
			//provider
			gazeDataInfo = new liblsl.StreamInfo("GazeData", "Gaze", 2, 70, liblsl.channel_format_t.cf_double64, "tobiieyex");
			gazeDataOutlet = new liblsl.StreamOutlet(gazeDataInfo);

			fixationBeginInfo = new liblsl.StreamInfo("FixationBegin", "Gaze", 2, 70, liblsl.channel_format_t.cf_double64, "tobiieyex");
			fixationBeginOutlet = new liblsl.StreamOutlet(fixationBeginInfo);

			fixationDataInfo = new liblsl.StreamInfo("FixatioData", "Gaze", 2, 70, liblsl.channel_format_t.cf_double64, "tobiieyex");
			fixationDataOutlet = new liblsl.StreamOutlet(fixationDataInfo);

			fixationEndInfo = new liblsl.StreamInfo("FixationEnd", "Gaze", 2, 70, liblsl.channel_format_t.cf_double64, "tobiieyex");
			fixationEndOutlet = new liblsl.StreamOutlet(fixationEndInfo);

			//intlet
			gazeDataResultsInfo = liblsl.resolve_stream("name", "GazeData");
			gazeDataInlet = new liblsl.StreamInlet(gazeDataResultsInfo[0]); ;

			fixationBeginResultsInfo = liblsl.resolve_stream("name", "FixationBegin");
			fixationBeginInlet = new liblsl.StreamInlet(fixationBeginResultsInfo[0]); ;

			fixationDataResultsInfo = liblsl.resolve_stream("name", "FixationData");
			fixationDataInlet = new liblsl.StreamInlet(fixationDataResultsInfo[0]); ;

			fixationEndResultsInfo = liblsl.resolve_stream("name", "FixationEnd");
			fixationEndInlet = new liblsl.StreamInlet(fixationEndResultsInfo[0]); ;



		}

		//send data to LSL
		public void sendGazeToLSL(liblsl.StreamOutlet outlet, double x, double y)
		{
			double[] data = { x, y };
			outlet.push_sample(data);
		}

		//calibrate eye tracker
		public void calibrate()
        {
			Console.WriteLine("Calibrating eye tracker");
            _host.Context.LaunchConfigurationTool(Tobii.Interaction.Framework.ConfigurationTool.Recalibrate, (data) => { });
			
        }

		public void readStreams()
		{
			_fixationDataStream
				.Begin((x, y, timestamp) =>
				{
					sendGazeToLSL(fixationBeginOutlet, x, y);
				})
				 .Data((x, y, timestamp) =>
				 {
					 sendGazeToLSL(fixationDataOutlet, x, y);

				 })
				.End((x, y, timestamp) =>
				{
					sendGazeToLSL(fixationEndOutlet, x, y);
				});

			_gazePointDataStream.GazePoint((x, y, timestamp) =>
			{
				sendGazeToLSL(gazeDataOutlet, x, y);
			});
		}

		public void recordStreams()
		{
			float[] sample = new float[2];
			while (true)
			{
				gazeDataInlet.pull_sample(sample);
				foreach (float f in sample)
					System.Console.Write("\t{0}", f);
				System.Console.WriteLine();
			}

		}

		//read fixations from eye tracker stream
		//      public void readFixationStream()
		//      {
		//	store fixation

		//	Fixation f = new Fixation();
		//	long startTime = -1;
		//	long endTime = -1;

		//	TODO send streams to LSL and perform cleaning of fixation data after receieving back from LSL
		//	_fixationDataStream
		//              .Begin((x, y, timestamp) =>
		//          {
		//		f.startPoint = new DataPoint(x, y, timestamp);
		//		Console.WriteLine("Fixation started at X:{0} Y:{1} Device timestamp: {2}", x, y, timestamp);
		//		startTime = Stopwatch.GetTimestamp();
		//	})
		//               .Data((x, y, timestamp) =>
		//          {
		//		f.points.Add(new DataPoint(x, y, timestamp));
		//		Console.WriteLine("During fixation at X:{0} Y:{1} Device timestamp: {2}", x, y, timestamp);

		//	})
		//              .End((x, y, timestamp) =>
		//          {
		//		endTime = Stopwatch.GetTimestamp();
		//		f.endPoint = new DataPoint(x, y, timestamp);
		//		if ((Double.IsNaN(f.endPoint.rawX) && Double.IsNaN(f.endPoint.rawY)) || startTime == -1)
		//		{
		//			Console.WriteLine("NOT A VALID FIXATION, DISCARDED");
		//		}
		//		else
		//		{
		//			Console.WriteLine("Fixation started at " + f.startPoint.timeStamp);
		//			Console.WriteLine("Fixation finished at X:{0} Y:{1} Device timestamp: {2}", x, y, timestamp);
		//			session.currentTestResults.fixationData.Add(f);
		//		}
		//		f.duration = endTime - startTime;
		//		f.completeFixation();
		//		findSaccade();
		//		f = new Fixation();
		//	});

		//	all this block of code does is print out the current fixation to thte console
		//          if (!Double.IsNaN(f.startPoint.rawX) && !Double.IsNaN(f.startPoint.rawY) && !Double.IsNaN(f.endPoint.rawX) && !Double.IsNaN(f.endPoint.rawY))
		//          {
		//              f.completeFixation();
		//              //Console.WriteLine(f.startPoint.X);
		//              //Console.WriteLine("duration {0})", f.duration);
		//              if (f.startPoint.timeStamp != 0)
		//              {
		//                  session.currentTestResults.fixationData.Add(f);
		//                  Console.WriteLine("NEW FIXATION: \n" +
		//                  "xPos:{0}\n" +
		//                  "yPos:{1}\n" +
		//                  "timestamp:{2}\n" +
		//                  "duration:{3}\n" +
		//                  "startPoint:{4}\n" +
		//                  "subPoints:{5}\n" +
		//                  "endPoint:{6}\n", f.centroid.rawX, f.centroid.rawY, f.startPoint.timeStamp, f.duration, f.startPoint.toString(), f.points.ToString(), f.endPoint.toString());
		//                  //attemptSaccade();
		//                  f = new Fixation();
		//              }
		//          }
		//          });
		//      }

		//read gaze data from eye tracker stream
		//public void readGazeStream()
  //      {
  //          _gazePointDataStream.GazePoint((x, y, timestamp) =>
  //          {
		//		sendGazeToLSL(gazeDataOutlet, x, y);

		//		//session.currentTestResults.rawGazeData.Add(new DataPoint(x, y, timestamp));
		//		////Console.WriteLine("NEW GAZE POINT: \n xPos: " + x + "\n yPos: " + y + "\n timestamp " + timestamp);
		//		//if(session.currentTestResults.startTime == -1)
		//		//{
		//		//    session.currentTestResults.startTime = timestamp; 
		//		//}
		//		//if(timestamp > session.currentTestResults.endTime)
		//		//{
		//		//    session.currentTestResults.endTime = timestamp;
		//	//}

		//	});
  //      }

		//calculate and record fixation
        //private void findSaccade()
        //{
        //    //store saccade
            
        //    if (session.currentTestResults.fixationData.Count > 1)
        //    {
        //        Saccade s = new Saccade();
        //        Fixation f1 = ((Fixation)session.currentTestResults.fixationData[session.currentTestResults.fixationData.Count - 2]);
        //        Fixation f2 = ((Fixation)session.currentTestResults.fixationData[session.currentTestResults.fixationData.Count - 1]);
        //        s.X1 = f1.centroid.rawX;
        //        s.Y1 = f1.centroid.rawY;
        //        s.X2 = f2.centroid.rawX;
        //        s.Y2 = f2.centroid.rawY;
        //        s.hlength = s.X2 - s.X1;
        //        s.vlength = s.Y2 - s.Y1;
        //        s.size = Math.Sqrt(Math.Pow(s.hlength, 2) + Math.Pow(s.vlength, 2));
        //        //s.start = f1.endPoint.timeStamp;
        //        //s.end = f2.startPoint.timeStamp;
        //        //s.duration = f2.startPoint.timeStamp - f1.endPoint.timeStamp;
        //        session.currentTestResults.SaccadeData.Add(s);

        //        Console.WriteLine("NEW SACCADE: \n" +
        //        "size:{0}\n" +
        //        "duration:{1}\n" +
        //        "startTime:{2}\n" +
        //        "endTime:{3}\n" +
        //        "x1Pos:{4}\n" +
        //        "y1Pos:{5}\n" +
        //        "x2Pos:{6}\n" +
        //        "y2Pos:{7}\n", s.size, s.duration, s.start, s.end, s.X1, s.Y1, s.X2, s.Y2);

        //    }
        //}

		
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

        public DataPoint(double X, double Y)
        {
            this.rawX = X;
            this.rawY = Y;
            this.timeStamp = -1;
            this.duration = -1;
        }

        public DataPoint(double X, double Y, double timeStamp)
        {
            this.rawX = X;
            this.rawY = Y;
            this.timeStamp = timeStamp;
            this.duration = -1;
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



