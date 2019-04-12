using GazeTrackingAttentionDemo.Models;
using GazeTrackingAttentionDemo.UserControls;
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
using static GazeTrackingAttentionDemo.MainWindow;

namespace GazeTrackingAttentionDemo
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlWindow : Window
    {
		System.Windows.Controls.UserControl newUserForm = new UserControls.UserCtrl();
		System.Windows.Controls.UserControl webcam = new UserControls.WebcamCtrl();
		MainWindow mainWin = (MainWindow)System.Windows.Application.Current.MainWindow;


		private WebcamCtrl _webcam;

		public ControlWindow()
		{
			Screen s1 = Screen.AllScreens[1];
			System.Drawing.Rectangle r1 = s1.WorkingArea;
			this.Top = r1.Top;
			this.Left = r1.Left;
			this.Width = r1.Width;
			this.Height = r1.Height;

			InitializeComponent();

			this.DataContext = this;
			controller.Content = newUserForm;

		}

		private void onLoad(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Maximized;
			this.KeyDown += new System.Windows.Input.KeyEventHandler(mainWin.MainWindow_KeyDown);
			mainWin.readyToRecord += new readyToRecordHandler(readyToRecord);

			mainWin.recordingStarted += new recordingStartHandler(startRecording);

			mainWin.recordingStopped += new recordingEndHandler(stopRecording);

			mainWin.progStateChanged += new stateChangedHandler(stateChanged);

		}

		public void stateChanged(EState state)
		{
			stateLabel.Content = state.ToString().ToUpper();
		}

		public void readyToRecord(Test test)
		{
			_webcam.setup(test.TestDir);
			_webcam.startPreview();
		}

		public void startRecording()
		{
			_webcam.startRecording();
		}
		public void stopRecording()
		{
			_webcam.stopRecording();
		}

		public void onUserCreated(User user)
		{
			controller.Content = webcam;
			_webcam = (WebcamCtrl)webcam;
		}





	}
}
