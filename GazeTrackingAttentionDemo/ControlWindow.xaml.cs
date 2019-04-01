using Microsoft.Expression.Encoder.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GazeTrackingAttentionDemo
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlWindow : Window
    {

		public Collection<EncoderDevice> VideoDevices { get; set; }
		public Collection<EncoderDevice> AudioDevices { get; set; }
		public ControlWindow()
        {
			Screen s1 = Screen.AllScreens[0];
			System.Drawing.Rectangle r1 = s1.WorkingArea;
			this.Top = r1.Top;
			this.Left = r1.Left;
			this.Width = r1.Width;
			this.Height = r1.Height;

			VideoDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);
			AudioDevices = EncoderDevices.FindDevices(EncoderDeviceType.Audio);

			this.DataContext = this;

			InitializeComponent();

		}

		private void onLoad(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Maximized;
			WebcamViewer.StartPreview();

		}

		private void Record_Click(object sender, RoutedEventArgs e)
		{
			WebcamViewer.StartRecording();
		}

		private void Stop_Click(object sender, RoutedEventArgs e)
		{
			WebcamViewer.StopRecording();
		}



	}
}
