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
	/// Interaction logic for MarkupCtrl.xaml
	/// </summary>
	public partial class MarkupCtrl : UserControl
	{

		Boolean showGaze;
		Boolean showFixations;
		Boolean showSaccades;
		Boolean dataRecorded;

		public MarkupCtrl()
		{
			InitializeComponent();

			this.DataContext = this;

			//Render.IsEnabled = false;
			//Begin.IsEnabled = true;
			//End.IsEnabled = false;
			//Test_Callibration.IsEnabled = true;
			//Callibrate.IsEnabled = true;
			//Reset.IsEnabled = true;
			//mainCanvas.Children.Clear();
		}

		private void btnPlay_Click(object sender, RoutedEventArgs e)
		{

			player.Play();
		}

		private void btnPause_Click(object sender, RoutedEventArgs e)
		{
			player.Pause();
		}

		private void btnStop_Click(object sender, RoutedEventArgs e)
		{
			player.Stop();
		}

		private void Render_Click(object sender, RoutedEventArgs e)
		{
			//DisplaySlider.Value = DisplaySlider.Maximum;
			//DisplaySlider.Visibility = Visibility.Visible;
			//DisplaySlider_Value.Visibility = Visibility.Visible;
			//All_CheckBox.Visibility = Visibility.Visible;
			//Saccade_CheckBox.Visibility = Visibility.Visible;
			//Fixation_CheckBox.Visibility = Visibility.Visible;
			//Gaze_CheckBox.Visibility = Visibility.Visible;
			//Visualisation_Header.Visibility = Visibility.Visible;
			//render();
		}

		private void Gaze_CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			showGaze = true;
		}

		private void Gaze_CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			showGaze = false;
		}

		private void Fixation_CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			showFixations = true;
		}

		private void Fixation_CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			showFixations = false;
		}

		private void Saccade_CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			showSaccades = true;
		}

		private void Saccade_CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			showSaccades = false;
		}

		private void All_CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			Saccade_CheckBox.IsChecked = true;
			Fixation_CheckBox.IsChecked = true;
			Gaze_CheckBox.IsChecked = true;
		}

		private void All_CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			Saccade_CheckBox.IsChecked = false;
			Fixation_CheckBox.IsChecked = false;
			Gaze_CheckBox.IsChecked = false;
		}

		private void CheckBox_Clicked(object sender, RoutedEventArgs e)
		{
			//    render();
		}
	}
}
