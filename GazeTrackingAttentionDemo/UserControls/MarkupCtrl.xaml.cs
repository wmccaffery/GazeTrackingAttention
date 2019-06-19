using GazeTrackingAttentionDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
	/// Interaction logic for MarkupCtrl.xaml
	/// </summary>
	public partial class MarkupCtrl : UserControl
	{

		Boolean showGaze;
		Boolean showFixations;
		Boolean showSaccades;
		Boolean dataRecorded;

		private MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;


		Test test;
		AOI aoi;
		Recording recording;

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

		public void onLoad(object sender, RoutedEventArgs e)
		{
			if (test == null)
			{
				timeline_group.IsEnabled = false;
				visualisation_group.IsEnabled = false;
				playback_group.IsEnabled = false;
			}
			if(aoi == null)
			{
				markup_group.IsEnabled = false;
			}

		}

		public void onAOIChanged(AOI aoi)
		{
			this.aoi = aoi;
			markup_group.IsEnabled = true;

		}

		public void onRecordingChanged(Recording recording)
		{
			this.recording = recording;

			if (recording != null)
			{
				timeline_group.IsEnabled = true;
				visualisation_group.IsEnabled = true;
				string[] video = Directory.GetFiles(recording.dataDir, "*.wmv");
				if (video.Length == 1)
				{
					player.Source = new Uri(video[0]);
					playback_group.IsEnabled = true;
				}

				if (test.currentRecording.fixations.Count > 0)
				{
					DisplaySlider.Maximum = recording.fixations[test.currentRecording.fixations.Count - 1].endPos.timestamp;
					DisplaySlider.Minimum = recording.fixations[0].startPos.timestamp;
				}
				else
				{
					DisplaySlider.Maximum = 0;
					DisplaySlider.Minimum = 0;
				}

				DisplaySlider.HigherValue = DisplaySlider.Maximum;
				DisplaySlider.LowerValue = DisplaySlider.Minimum;
			}
		}

		public void onTestChanged(Test test)
		{
			this.test = test;
			this.aoi = null;
			markup_group.IsEnabled = false;

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

		//re-render gaze plot if checkbox or slider changed
		private void CheckBox_Clicked(object sender, RoutedEventArgs e)
		{
			if (test.currentRecording.gp != null)
			{
				test.currentRecording.gp.renderPlot((bool)All_CheckBox.IsChecked, (bool)Fixation_CheckBox.IsChecked, (bool)Saccade_CheckBox.IsChecked, DisplaySlider.LowerValue, DisplaySlider.HigherValue);
			}
		}

		private void DisplaySlider_LowerValueChanged(object sender, RoutedEventArgs e)
		{
			if (test.currentRecording.gp != null)
			{
				test.currentRecording.gp.renderPlot((bool)All_CheckBox.IsChecked, (bool)Fixation_CheckBox.IsChecked, (bool)Saccade_CheckBox.IsChecked, DisplaySlider.LowerValue, DisplaySlider.HigherValue);
			}
		}

		private void DisplaySlider_HigherValueChanged(object sender, RoutedEventArgs e)
		{
			if (test.currentRecording.gp != null)
			{
				test.currentRecording.gp.renderPlot((bool)All_CheckBox.IsChecked, (bool)Fixation_CheckBox.IsChecked, (bool)Saccade_CheckBox.IsChecked, DisplaySlider.LowerValue, DisplaySlider.HigherValue);
			}
		}

		private void RadioBtn_Checked(object sender, RoutedEventArgs e)
		{
			String group = ((RadioButton)e.Source).GroupName;
			int val = Int32.Parse(((RadioButton)e.Source).Content.ToString());
		}

		private void AddAnnotation_Click(object sender, RoutedEventArgs e)
		{
			if(aoi != null) {
				writeToJSON(aoi);
			}
			
		}

		public void writeToJSON(AOI aoi) //adapted from stackoverflow https://stackoverflow.com/questions/20626849/how-to-append-a-json-file-without-disturbing-the-formatting
		{
			string filePath = recording.dataDir + "\\annotations.json";
			List<AOI> aoiList;
			String jsonData;

			// Read existing json data
			if (!File.Exists(filePath))
			{
				File.WriteAllText(filePath, "");
				aoiList = new List<AOI>();
			}
			else
			{

				jsonData = File.ReadAllText(filePath);

				// De-serialize to object or create new list
				aoiList = JsonConvert.DeserializeObject<List<AOI>>(jsonData)
					  ?? new List<AOI>();
			}

			// Add new aoi
			bool alreadyAnnotated = false;
			int index = 0;
			foreach (AOI a in aoiList)
			{
				if (Equals(aoi.Name, a.Name) && Equals(aoi.location, a.location))
				{
					index = aoiList.IndexOf(a);
					alreadyAnnotated = true;
				}
			}

			if (alreadyAnnotated)
			{
				aoiList[index] = aoi;
			}
			else
			{
				aoiList.Add(aoi);
			}


			// Update json data string
			jsonData = JsonConvert.SerializeObject(aoiList);

			System.IO.File.WriteAllText(filePath, jsonData);

		}
	}
}
