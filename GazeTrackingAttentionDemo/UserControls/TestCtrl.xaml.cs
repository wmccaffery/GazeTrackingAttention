using GazeTrackingAttentionDemo.Models;
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
		private double ustartts;

		public delegate void TestCompletedHandler();
		public event TestCompletedHandler TestCompleted;

		public delegate void StreamingHandler();
		public event StreamingHandler Streaming;

		public delegate void NotStreamingHandler();
		public event NotStreamingHandler NotStreaming;

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
			NotStreaming += new NotStreamingHandler(_mainWindow.stopStreaming);


		}

		public void startStreaming()
		{
			user.CurrentTest.dataRecorder.startStreaming();
			WebcamViewer.StartPreview();
			Streaming();
		}

		public void stopStreaming()
		{
			user.CurrentTest.dataRecorder.stopStreaming();
			WebcamViewer.StopPreview();
			NotStreaming();
		}

		public void startRecording()
		{
			user.CurrentTest.addNewRecording(user);
			user.CurrentTest.dataRecorder.setRecordingPaths(false);
			user.CurrentTest.dataRecorder.startRecording();
			WebcamViewer.StartRecording();
			startts = _mainWindow.stopwatch.ElapsedMilliseconds;
			ustartts = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

			//recordingStarted();
		}

		public void stopRecording()
		{
			String dataPath = user.CurrentTest.currentRecording.dataDir;
			DeviceInteraction.DeviceInteractionHost dr = user.CurrentTest.dataRecorder;

			dr.stopRecording();
			WebcamViewer.StopRecording();
			endts = _mainWindow.stopwatch.ElapsedMilliseconds;

			user.CurrentTest.currentRecording.fixations = dr.getCleanFixations();
			user.CurrentTest.currentRecording.saccades = dr.getSaccades(user.CurrentTest.currentRecording.fixations);
			user.CurrentTest.currentRecording.videoQpcStartTime = startts;
			user.CurrentTest.currentRecording.videoQpcEndTime = endts;

			String fixationpath = dr.getCleanFixationPath();

			//write fixations
			foreach (Fixation f in user.CurrentTest.currentRecording.fixations)
			{
				dr.writeCleanFixationsToFile(fixationpath, f);
			}

			////set highest test
			if (user.CurrentTest.index > user.highestTestIndex)
			{
				user.highestTestIndex = user.CurrentTest.index;
			}


			string[] video = Directory.GetFiles(WebcamViewer.VideoDirectory, "*.wmv");
			String dirdata = System.IO.Path.GetFileName(user.CurrentTest.DataDir);
			File.Move(video[0], user.CurrentTest.DataDir + "//" + dirdata + "_U" + ustartts + "_VIDEO_" + "_Q" + startts + "_Q" + endts + ".wmv");
		}

		private void FinishTest_Click(object sender, RoutedEventArgs e)
		{
			if(_mainWindow.State == MainWindow.EState.Streaming)
			{
				stopStreaming();
			}
			user.CurrentTest.testComplete();
			TestCompleted();
		}

		private void Stream_Click(object sender, RoutedEventArgs e)
		{
			if (_mainWindow.State == MainWindow.EState.Ready)
			{
				startStreaming();
				record.IsEnabled = true;
				stream.Content = "Stop Streaming";

			}
			else if (_mainWindow.State == MainWindow.EState.Streaming)
			{
				stopStreaming();
				record.IsEnabled = false;
				stream.Content = "Stream";

			}

		}

		private void Record_Click(object sender, RoutedEventArgs e)
		{
			if (_mainWindow.State == MainWindow.EState.Streaming)
			{
				startRecording();
				stream.IsEnabled = false;
				finishTest.IsEnabled = false;
				((Button)e.Source).Content = "Stop Recording";
				_mainWindow.State = MainWindow.EState.Recording;

			}
			else if (_mainWindow.State == MainWindow.EState.Recording)
			{

				stopRecording();
				stream.IsEnabled = true;
				finishTest.IsEnabled = true;
				((Button)e.Source).Content = "Start Recording";
				_mainWindow.State = MainWindow.EState.Streaming;
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
