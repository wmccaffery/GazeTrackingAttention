﻿using GazeTrackingAttentionDemo.Models;
using Microsoft.Expression.Encoder.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GazeTrackingAttentionDemo.UserControls
{
	/// <summary>
	/// Interaction logic for WebcamCtrl.xaml
	/// </summary>
	public partial class TestCtrl : UserControl
	{ 

		public Collection<EncoderDevice> VideoDevices { get; set; }

		public EncoderDevice logiHdPro { get; set; }

		private MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;
		private ControlWindow _parentWin;
		User user;

		private double startts;
		private double endts;

		public delegate void TestCompletedHandler();
		public event TestCompletedHandler TestCompleted;

		public delegate void StreamingHandler();
		public event StreamingHandler Streaming;

		public TestCtrl()
		{

			VideoDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);

			foreach (EncoderDevice d in VideoDevices)
			{
				if (d.Name == "HD Pro Webcam C920" || d.Name == "Integrated Webcam")
				{
					Console.WriteLine("Webcam found");
					logiHdPro = d;
				}
				Console.WriteLine("VIDEO DEVICE: " + d.Name);
			}

			if(logiHdPro == null)
			{
				Console.WriteLine("No recognized webcams available");
			}


			this.DataContext = this;

			InitializeComponent();
		}


		private void onLoad(object sender, RoutedEventArgs e)
		{

			_parentWin = (ControlWindow)Window.GetWindow(this);
			user = _parentWin.user;

			//WebcamViewer.StartPreview();
			TestCompleted += new TestCompletedHandler(_mainWindow.testCompleted);
			Streaming += new StreamingHandler(_mainWindow.startStreaming);

		}

		public void startRecording()
		{
			user.CurrentTest.dataRecorder.recordStreams();
			WebcamViewer.StartRecording();
			startts = _mainWindow.stopwatch.ElapsedMilliseconds;
			//recordingStarted();
		}

		public void stopRecording()
		{
			WebcamViewer.StopRecording();
			endts = _mainWindow.stopwatch.ElapsedMilliseconds;
			WebcamViewer.StopPreview();
			string[] video = Directory.GetFiles(WebcamViewer.VideoDirectory, "*.wmv");
			File.Move(video[0], user.CurrentTest.DataDir + "//Subject" + _mainWindow.currentUser.Id  + "QPCstart" + startts + "end" + endts + ".wmv");
		}

		private void FinishTest_Click(object sender, RoutedEventArgs e)
		{
			user.CurrentTest.testComplete();
			TestCompleted();
		}

		private void Stream_Click(object sender, RoutedEventArgs e)
		{
			user.CurrentTest.addNewRecording(user);
			user.CurrentTest.dataRecorder.readStreams(false);
			WebcamViewer.StartPreview();
			Streaming();
		}

		private void Record_Click(object sender, RoutedEventArgs e)
		{
			if(_mainWindow.State == MainWindow.EState.Recording)
			{
				String dataPath = user.CurrentTest.currentRecording.dataDir;
				DataProcessing.StreamReader dr = user.CurrentTest.dataRecorder;
				dr.stopRecording();
				dr.closeLslStreams();
				stopRecording();

				user.CurrentTest.currentRecording.fixations = dr.getFixations();

				//gen cleaned fixation file pathname
				String testDir = dataPath;
				String uid = user.Id;
				int test = user.CurrentTest.index;
				DateTime time = DateTime.Now;
				Int32 unixts = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
				String rawPath = testDir + "//" + uid + "_test_" + test + time.ToString("dd-MM-yyyy--HH-mm-ss") + "_U" + unixts + "_EYETRACKER_cleanFixationData.csv";

				//write fixations
				foreach (Fixation f in user.CurrentTest.currentRecording.fixations)
				{
					dr.recordFixations(rawPath, f);
				}

				//set highest test
				if (user.CurrentTest.index > user.highestTestIndex)
				{
					user.highestTestIndex = user.CurrentTest.index;
				}
				((Button)e.Source).Content = "Start Recording";
				_mainWindow.State = MainWindow.EState.Ready;

			}
			else if(_mainWindow.State == MainWindow.EState.Streaming)
			{
				startRecording();
				((Button)e.Source).Content = "Stop Recording";
				_mainWindow.State = MainWindow.EState.Recording;
			}
		}

		private void StartCalibration_Click(object sender, RoutedEventArgs e)
		{
			user.CurrentTest.dataRecorder.calibrate();
			_mainWindow.State = MainWindow.EState.Calibrating;
		}

		//private void Record_Click(object sender, RoutedEventArgs e)
		//{
		//	WebcamViewer.StartRecording();
		//	//get timestamp
		//}

		//private void Stop_Click(object sender, RoutedEventArgs e)
		//{
		//	WebcamViewer.StopRecording();
		//}
	}
}
