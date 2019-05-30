﻿
using GazeTrackingAttentionDemo.DataVisualization;
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
		public enum EState { Wait, Setup, Test, ReadyToCalibrate, Ready, Streaming, Recording, DoneRecording, Selection, ActiveSelection, Markup }
		//Wait: time between tests
		//Setup: user creation and initial calibration
		//Test: testing user calibration
		//ReadyToRecord: ready to start recording data
		//Recording: data is being recorded
		//DoneRecording: data has been recorded
		//Selection: loaded interface for selecting datapoints for paragraphs
		//ActiveSelection: in the process of selecting datapoints
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
		UserControl selectionctrl = new SelectionCtrl();
		UserControl test = new UserControls.TestCalibrationCtrl();
		UserControl markup = new UserControls.MarkupCtrl();
		ControlWindow ctrlwin;

		public Stopwatch stopwatch = new Stopwatch();



		//current user data
		public User currentUser;

		//all tests
		public List<Test> testInstances = new List<Test>();

		//current test
		private Test _currentTestInstance = null;
		public Test currentTestInstance
		{
			get { return _currentTestInstance; }
			set
			{
				if(_currentTestInstance != null) //add previous value to list
				{
					testInstances.Add(_currentTestInstance);
				}
				_currentTestInstance = value;
			}
		}

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

		public delegate void testChangedHandler(String path);
		public event testChangedHandler currentTestChanged;

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
			//this.progStateChanged += new stateChangedHandler(stateChanged);

		}



		//handle user being created in ControlWindow
		public void onUserCreated(User user)
		{
			centerView.Content = document;

			currentUser = user;
			State = EState.ReadyToCalibrate;

			//load test files
			filePaths = new List<String>(Directory.GetFiles(currentUser.GroupPath));
			filePaths.Sort((String a, String b) =>
			{
				//sort files into order based on number in title
				int a_num = Int32.Parse(Regex.Match(a, @"\d+").Value);
				int b_num = Int32.Parse(Regex.Match(b, @"\d+").Value);
				//Console.WriteLine("ANUM " + a_num);
				//Console.WriteLine("BNUM " + b_num);

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
				currentTestChanged(filePaths[testIndex]);
				return true;
			}
			return false;
		}

		public String getCurrentTest()
		{
			return filePaths[testIndex];
		}
		
		//Handle user keyboard input TODO delegate actions to state handler
		public void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.R:
					//Restart current test
					Console.WriteLine("Restart command has not yet been implemented");
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
					//if(State == EState.ReadyToCalibrate || State == EState.DoneRecording)
					//{
					//	if (!incrementTestCounter())
					//	{
					//		//recrding phase complete
					//		//System.Windows.Application app = System.Windows.Application.Current;
					//		//Application.Current.Shutdown();

					//		GazePlot gp = new GazePlot(currentTestInstance.cleanedFixations, MainCanvas);
					//		gp.renderPlot(true, false, false, false, 0, 999999);
					//		testIndex = 0;
					//		loadTest(filePaths[testIndex]);
					//		State = EState.Selection;


					//		//State = EState.Markup;
					//	} else
					//	{
					//		((DocumentCtrl)document).clearText();
					//		Console.WriteLine("Starting test " + testIndex);
					//		currentTestInstance = new Test(currentUser, filePaths[testIndex], testIndex);
					//		currentTestInstance.fd.calibrate();
					//		State = EState.Ready;
					//	}
					//}
					//break;
					switch (State)
					{
						case EState.ReadyToCalibrate:
							startTest();
							break;

						case EState.DoneRecording:
							if (!incrementTestCounter())
							{
								testInstances.Add(currentTestInstance); //save final test instance
								testIndex = 0;
								loadTest(filePaths[testIndex]); //load original test instance
								GazePlot gp = new GazePlot(currentTestInstance.cleanedFixations, MainCanvas); //red
								gp.renderPlot(true, false, false, false, 0, 999999);
								State = EState.Selection;
							}
							else
							{
								startTest();
							}
							break;

						case EState.Selection:
							if (!incrementTestCounter())
							{
								GazePlot gp = new GazePlot(currentTestInstance.cleanedFixations, MainCanvas);
								gp.renderPlot(true, false, false, false, 0, 999999);
								testIndex = 0;
								//currentTestInstance = 
								//State = EState.Markup;
							}
							else
							{
								((DocumentCtrl)document).clearText();
								Console.WriteLine("test " + testIndex + " loaded for selection");
								currentTestInstance = new Test(currentUser, filePaths[testIndex], testIndex);
							}
							break;

						case EState.Markup:
							if (!incrementTestCounter())
							{
								GazePlot gp = new GazePlot(currentTestInstance.cleanedFixations, MainCanvas);
								gp.renderPlot(true, false, false, false, 0, 999999);
								testIndex = 0;
								loadTest(filePaths[testIndex]);
								State = EState.Markup;
							}
							else
							{
								((DocumentCtrl)document).clearText();
								Console.WriteLine("test " + testIndex + " loaded for selection");
								currentTestInstance = new Test(currentUser, filePaths[testIndex], testIndex);
							}
							break;
					}

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
						currentTestInstance.cleanedFixations = currentTestInstance.fd.cleanFixations();

						//gen cleaned fixation file pathname
						String testDir = currentTestInstance.TestDir;
						String uid = currentTestInstance.User;
						int test = currentTestInstance.index;
						DateTime time = DateTime.Now;
						Int32 unixts = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
						String rawPath = testDir + "//" + uid + "_test_" + test + time.ToString("dd-MM-yyyy--HH-mm-ss") + "_U" + unixts + "_EYETRACKER_cleanFixationData.csv";
						foreach (Fixation f in currentTestInstance.cleanedFixations)
						{
							currentTestInstance.fd.recordCleanedFixation(rawPath, f);
						}
						State = EState.DoneRecording;
					}
					break;
				default:
					break;
			}
		}

		public void startTest()
		{
			((DocumentCtrl)document).clearText();
			Console.WriteLine("Starting test " + testIndex);
			currentTestInstance = new Test(currentUser, filePaths[testIndex], testIndex);
			currentTestInstance.fd.calibrate();
			State = EState.Ready;
		}

		public void startSelection(String paragraph)
		{
			State = EState.ActiveSelection;
		}

		public void endSelection(String paragraph)
		{
			State = EState.Selection;
		}

		//Handle statechange
		//public void stateChanged(EState state)
		//{
		//	switch (State)
		//	{
		//		case EState.Streaming:

		//	}
		//}

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



