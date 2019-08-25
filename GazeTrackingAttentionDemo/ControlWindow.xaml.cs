using GazeTrackingAttentionDemo.Models;
using GazeTrackingAttentionDemo.UserControls;
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
		public System.Windows.Controls.UserControl newUserForm = new UserControls.UserCtrl();
		public System.Windows.Controls.UserControl testCtrl = new UserControls.TestCtrl();
		public System.Windows.Controls.UserControl calibrationCtrl = new UserControls.CalibrationCtrl();
		public System.Windows.Controls.UserControl overview = new UserControls.OverviewCtrl();
		public System.Windows.Controls.UserControl endCtrl = new UserControls.EndCtrl();

		MainWindow mainWin = (MainWindow)System.Windows.Application.Current.MainWindow;

		public User user;
		public int totalNumTests;

		Boolean multiscreen;

		public ControlWindow()
		{
			Screen controlScreen;

			if (Screen.AllScreens.Length > 1)
			{
				controlScreen = Screen.AllScreens[1];
				System.Drawing.Rectangle r1 = controlScreen.WorkingArea;
				this.Top = r1.Top;
				this.Left = r1.Left;
				this.Width = r1.Width;
				this.Height = r1.Height;
				multiscreen = true;
			}
			else
			{
				multiscreen = false;
			}
			InitializeComponent();

			this.DataContext = this;
			controller.Content = newUserForm;

		}

		private void onLoad(object sender, RoutedEventArgs e)
		{
			if (multiscreen)
			{
				this.WindowState = WindowState.Maximized;
			} else
			{
				this.WindowStyle = WindowStyle.SingleBorderWindow;
			}

			//this.KeyDown += new System.Windows.Input.KeyEventHandler(mainWin.MainWindow_KeyDown);
			//mainWin.readyToRecord += new readyToRecordHandler(readyToRecord); //used to start webcam

			//mainWin.progStateChanged += new stateChangedHandler(stateChanged);

		}

		public void stateChanged(EState state)
		{
			switch (state)
			{
				case EState.Overview:
					controller.Content = overview; 
					break;
				case EState.Ready:
					controller.Content = new TestCtrl();
					break;
				case EState.Calibrating:
					controller.Content = calibrationCtrl;
					break;
				case EState.Markup:
					controller.Content = endCtrl;
					break;
                case EState.Setup:
                    controller.Content = new UserCtrl();
                    break;
			}
			stateLabel.Content = state.ToString();

		}

		public void onUserCreated(User user)
		{
			totalNumTests = user.getTestPaths().Count;
			userLabel.Content = user.Id;
			this.user = user;
		}

		public void testCreated()
		{
			Test test = Session.currentTest;
			if (test.isPaper)
			{
				currentTestMedium.Content = "Paper";
			} else
			{
				currentTestMedium.Content = "Screen";
			}
			currentTestName.Content = System.IO.Path.GetFileNameWithoutExtension(test.StimuliPath);
		}
	}
}
