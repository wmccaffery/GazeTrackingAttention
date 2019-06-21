using System;
using System.Collections.Generic;
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
	/// Interaction logic for CalibrationCtrl.xaml
	/// </summary>
	public partial class CalibrationCtrl : UserControl
	{

		MainWindow _mainWin = (MainWindow)Application.Current.MainWindow;

		//public delegate void endCalibrationHandler();
		//public event endCalibrationHandler endCalibration;

		public CalibrationCtrl()
		{
			InitializeComponent();
		}

		private void onLoad(object sender, RoutedEventArgs e)
		{
			//	endCalibration += new endCalibrationHandler((_mainWin.ctrlwin.controller.ContentendCalibration);

		}

		private void DoneCalibrating_Click(object sender, RoutedEventArgs e)
		{
			_mainWin.State = MainWindow.EState.Ready;
		}
	}
}
