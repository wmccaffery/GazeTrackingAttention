
using GazeTrackingAttentionDemo.LSLInteraction;
using GazeTrackingAttentionDemo.Models;
using GazeTrackingAttentionDemo.UserControls;
using LSL;
using Microsoft.Expression.Encoder.Devices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Tobii.Interaction;
using Tobii.Interaction.Framework;
using Application = System.Windows.Application;
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


		private EState _state;
		public EState State// { get; set; }
		{
			get { return _state; }
			set
			{
				_state = value;
				progStateChanged(_state);
			}
		}



		UserControl document = new UserControls.DocumentCtrl();
		UserControl test = new UserControls.TestCalibrationCtrl();
		UserControl markup = new UserControls.MarkupCtrl();
		ControlWindow ctrlwin;

		public Stopwatch stopwatch = new Stopwatch();
		

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

		public delegate void readyToRecordHandler(Test test);
		public event readyToRecordHandler readyToRecord;

		public delegate void recordingStartHandler();
		public event recordingStartHandler recordingStarted;

		public delegate void recordingEndHandler();
		public event recordingEndHandler recordingStopped;

		public delegate void stateChangedHandler (EState state);
		public event stateChangedHandler progStateChanged;



		//init gaze stream
		//StreamReader fd;

		//Thread eegStream;

		//Session session;

		//collection of all video devices available
		public Collection<EncoderDevice> VideoDevices { get; set; }

		//public Collection<EncoderDevice> AudioDevices { get; set; }

		public MainWindow()
		{
			//start stopwatch for timestamps
			stopwatch.Start();
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
				ctrlwin = new ControlWindow();
				ctrlwin.Owner = this;
				ctrlwin.Show();
			};
			
			//readyToRecord += new readyToRecordHandler(ctrlwin.readyToRecord);

			//recordingStarted += new recordingStartHandler(ctrlwin.startRecording);

			//recordingStopped += new recordingEndHandler(ctrlwin.stopRecording);
		}


		public void onLoad(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Maximized;
			this.KeyDown += new System.Windows.Input.KeyEventHandler(MainWindow_KeyDown);

		}

		public void onUserCreated(User user)
		{
			centerView.Content = document;

			currentUser = user;
			State = EState.ReadyToCalibrate;

			//load test files
			filePaths = new List<String>(Directory.GetFiles(currentUser.GroupPath));
			filePaths.Sort((String a, String b) =>
			{
				int a_num = Int32.Parse(Regex.Match(a, @"\d+").Value);
				int b_num = Int32.Parse(Regex.Match(b, @"\d+").Value);
				Console.WriteLine("ANUM " + a_num);
				Console.WriteLine("BNUM " + b_num);

				return a_num.CompareTo(b_num);
			});
			testIndex = -1;
			Console.WriteLine("FP " + filePaths[0]);
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
			if (testIndex < filePaths.Count-1)
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
					if (State != EState.Wait)
					{
						Console.WriteLine("Exiting");
						Environment.Exit(0);
					}
					break;
				case Key.N:
					//load next test, when finished start markup
					if(State == EState.ReadyToCalibrate || State == EState.DoneRecording)
					{
						if (!incrementTestCounter())
						{
							//recrding phase complete
							System.Windows.Application app = System.Windows.Application.Current;
							Application.Current.Shutdown();
							
							//State = EState.Markup;
						} else
						{
							((DocumentCtrl)document).clearText();
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
						readyToRecord(currentTestInstance);

						Console.WriteLine("Started Reading streams to LSL");

						State = EState.Streaming;
					} else if (State == EState.Streaming)
					{
						currentTestInstance.fd.recordStreams();
						recordingStarted();
						State = EState.Recording;
					} else if (State == EState.Recording)
					{
						currentTestInstance.fd.stopRecording();
						recordingStopped();
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


		//private void DisplayTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
  //      {
  //          //displayTime = DisplaySlider.Value;
  //          //render();
  //      }	
	}
}



