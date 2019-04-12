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
	public partial class WebcamCtrl : UserControl
	{ 

		public Collection<EncoderDevice> VideoDevices { get; set; }

		public EncoderDevice logiHdPro { get; set; }

		private MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;

		private double startts;
		private double endts;
	
		public WebcamCtrl()
		{

			VideoDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);

			foreach (EncoderDevice d in VideoDevices)
			{
				if (d.Name == "HD Pro Webcam C920")
				{
					Console.WriteLine("Webcam found");
					logiHdPro = d;
				}
			}

			if(logiHdPro == null)
			{
				Console.WriteLine("HD Pro Webcam C920 not found");
			}


			this.DataContext = this;

			InitializeComponent();
		}


		private void onLoad(object sender, RoutedEventArgs e)
		{
			//WebcamViewer.StartPreview();
			
		}

		public void setup(String path)
		{
			WebcamViewer.VideoDirectory = path;
			Console.WriteLine("VID DIR SET TO " + WebcamViewer.VideoDirectory);
			string[] video = Directory.GetFiles(WebcamViewer.VideoDirectory, "*.wmv");
			foreach (String s in video)
			{
				File.Delete(s);
			}
		}

		public void startPreview()
		{
			WebcamViewer.StartPreview();

		}

		public void startRecording()
		{
			WebcamViewer.StartRecording();
			startts = _mainWindow.stopwatch.ElapsedMilliseconds;
		}

		public void stopRecording()
		{
			WebcamViewer.StopRecording();
			endts = _mainWindow.stopwatch.ElapsedMilliseconds;
			string[] video = Directory.GetFiles(WebcamViewer.VideoDirectory, "*.wmv");
			File.Move(video[0], WebcamViewer.VideoDirectory + "//Subject" + _mainWindow.currentUser.Id  + "QPCstart" + startts + "end" + endts + ".wmv");
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
