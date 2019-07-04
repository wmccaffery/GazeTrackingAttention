using GazeTrackingAttentionDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GazeTrackingAttentionDemo.UserControls
{
	/// <summary>
	/// Interaction logic for MarkupCtrl.xaml
	/// </summary>
	public partial class MarkupCtrl : UserControl
	{

		private MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;

		public delegate void endMarkupHandler();
		public event endMarkupHandler endMarkup;


		Test test;
		AOI aoi;
		Recording recording;

		double startTime;

		int effort;
		int attentiveness;
		int interest;

		private bool mediaPlayerIsPlaying = false;
		private bool userIsDraggingSlider = false;


		public MarkupCtrl()
		{
			InitializeComponent();

			this.DataContext = this;

			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(1);
			timer.Tick += timer_Tick;
			timer.Start();

		}

		#region adapted from https://wpf-tutorial.com/audio-video/how-to-creating-a-complete-audio-video-player/
		private void timer_Tick(object sender, EventArgs e)
		{
			if ((player.Source != null) && (player.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
			{
				sliProgress.Minimum = 0;
				sliProgress.Maximum = player.NaturalDuration.TimeSpan.TotalMilliseconds; // DisplaySlider.HigherValue - startTime
				sliProgress.Value = player.Position.TotalMilliseconds;
			}
		}

		private void btnPlay_Click(object sender, RoutedEventArgs e)
		{
			player.Play();
			mediaPlayerIsPlaying = true;
		}

		private void btnPause_Click(object sender, RoutedEventArgs e)
		{
			player.Pause();
		}

		private void btnStop_Click(object sender, RoutedEventArgs e)
		{
			player.Stop();
			mediaPlayerIsPlaying = false;
		}

		private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
		{
			userIsDraggingSlider = true;
			player.Pause();
		}

		private void sliProgress_DragDelta(object sender, DragDeltaEventArgs e)
		{
			player.Position = TimeSpan.FromMilliseconds(sliProgress.Value);
		}

		private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			userIsDraggingSlider = false;
			player.Position = TimeSpan.FromMilliseconds(sliProgress.Value);
			player.Play();
		}

		private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			lblProgressStatus.Text = TimeSpan.FromMilliseconds(sliProgress.Value).ToString(@"mm\:ss\.fff");
		}

		#endregion

		public void onLoad(object sender, RoutedEventArgs e)
		{
			//if (test == null)
			//{
			test = null;
			aoi = null;
			recording = null;
			effort = 0;
			attentiveness = 0;
			interest = 0;
			DisplaySlider.LowerValue = 0;
			DisplaySlider.HigherValue = 0;
			DisplaySlider.Maximum = 0;
			DisplaySlider.Minimum = 0;
			timeline_group.IsEnabled = false;
			visualisation_group.IsEnabled = false;
			playback_group.IsEnabled = false;
			foreach (UIElement element in ((Grid)markup_group.Content).Children)
			{
				if (Equals(element.GetType(), Attentiveness.GetType())) {
					foreach (UIElement innerelement in ((StackPanel)element).Children)
					{
						if (Equals(innerelement.GetType(), i1.GetType()))
						{
							((RadioButton)innerelement).IsChecked = false;
						}
					}
				}
			}
			//}
			//if(aoi == null)
			//{
				markup_group.IsEnabled = false;
			//}

			endMarkup += new endMarkupHandler(((EndCtrl)_mainWindow.ctrlwin.controller.Content).markupExitComplete);

		}

		public void onAOIChanged(AOI aoi)
		{
			if (aoi != null)
			{
				this.aoi = aoi;
				effort = aoi.Effort;
				attentiveness = aoi.Attentiveness;
				interest = aoi.Interest;
				setCheckBoxes();
				markup_group.IsEnabled = true;
			} else
			{
				markup_group.IsEnabled = false;
			}

		}

		public void onRecordingChanged(Recording recording)
		{
			if (recording != null)
			{
				this.recording = recording;
				timeline_group.IsEnabled = true;
				visualisation_group.IsEnabled = true;
				string[] video = Directory.GetFiles(recording.dataDir, "*.wmv");
				if (video.Length == 1)
				{
					player.Source = new Uri(video[0]);
					playback_group.IsEnabled = true;
				}

				//resolveTimes();

				if (test.currentRecording.fixations.Count > 0)
				{
					DisplaySlider.Maximum = recording.fixations[test.currentRecording.fixations.Count - 1].endPos.timestamp;
					startTime = DisplaySlider.Minimum = recording.fixations[0].startPos.timestamp;
				}
				else
				{
					startTime = DisplaySlider.Maximum = 0;
					DisplaySlider.Minimum = 0;
				}

				DisplaySlider.HigherValue = DisplaySlider.Maximum;
				DisplaySlider.LowerValue = DisplaySlider.Minimum;

			} 
			aoi = null;
			markup_group.IsEnabled = false;
		}

		//decides to use video or fixation recording start and end times to account for small differences
		private void resolveTimes()
		{
			double fst = DisplaySlider.Minimum = recording.fixations[0].startPos.timestamp;
			double vst = recording.videoQpcStartTime;

			double fet = recording.fixations[test.currentRecording.fixations.Count - 1].endPos.timestamp;
			double vet = recording.videoQpcEndTime;

			//set displayslider, rounding is performed as large decimals make the slider unusable, and does not affect the ability to select data.
			if (fst < vst)
			{
				DisplaySlider.Minimum = fst; 
			}
			else
			{
				DisplaySlider.Minimum = vst;
			}
			if (fet > vet)
			{
				DisplaySlider.Maximum = fet;
			} else
			{
				DisplaySlider.Maximum = vet;
			}
			startTime = DisplaySlider.Minimum;

			//set video



		}



		public void onTestChanged(Test test)
		{
			this.test = test;
			if (this.aoi != null)
			this.aoi = null;
			markup_group.IsEnabled = false;
			timeline_group.IsEnabled = false;
			visualisation_group.IsEnabled = false;

		}

		private void All_CheckBox_Checked(object sender, RoutedEventArgs e) //all of the checkboxes should be written to properly use this
		{
			Saccade_CheckBox.IsChecked = true;
			Fixation_CheckBox.IsChecked = true;
			Aoi_CheckBox.IsChecked = true;
			_mainWindow.SelectionCanvas.Visibility = Visibility.Visible;

		}

		private void All_CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			Saccade_CheckBox.IsChecked = false;
			Fixation_CheckBox.IsChecked = false;
			Aoi_CheckBox.IsChecked = false;
			_mainWindow.SelectionCanvas.Visibility = Visibility.Hidden;

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
			if (test != null)
			{
				if (test.currentRecording.gp != null)
				{
					test.currentRecording.gp.renderPlot((bool)All_CheckBox.IsChecked, (bool)Fixation_CheckBox.IsChecked, (bool)Saccade_CheckBox.IsChecked, DisplaySlider.LowerValue, DisplaySlider.HigherValue);
				}
				if (aoi != null)
				{
					aoi.timeRangeStart = ((Xceed.Wpf.Toolkit.RangeSlider)e.Source).LowerValue;
					writeToJSON(aoi);
				}
			}
			TimeRange_Start.Text = String.Format("{0:0}",("" + (DisplaySlider.LowerValue - startTime)));
		}

		private void DisplaySlider_HigherValueChanged(object sender, RoutedEventArgs e)
		{
			if (test != null)
			{
				if (test.currentRecording.gp != null)
				{
					test.currentRecording.gp.renderPlot((bool)All_CheckBox.IsChecked, (bool)Fixation_CheckBox.IsChecked, (bool)Saccade_CheckBox.IsChecked, DisplaySlider.LowerValue, DisplaySlider.HigherValue);
				}
				if (aoi != null)
				{
					aoi.timeRangeEnd = ((Xceed.Wpf.Toolkit.RangeSlider)e.Source).HigherValue;
					writeToJSON(aoi);
				}
			}
			TimeRange_End.Text = String.Format("{0:0}", ("" + (DisplaySlider.HigherValue - startTime)));
		}

		private void RadioBtn_Checked(object sender, RoutedEventArgs e)
		{
			String group = ((RadioButton)e.Source).GroupName;
			int val = Int32.Parse(((RadioButton)e.Source).Content.ToString());
			switch (group)
			{
				case "Interest":
					interest = val;
					break;
				case "Attentiveness":
					attentiveness = val;
					break;
				case "Effort":
					effort = val;
					break;
			}
			writeToJSON(aoi);
			
		}

		public void setCheckBoxes()
		{
			UIElementCollection ae = Attentiveness.Children;
			UIElementCollection ee = Effort.Children;
			UIElementCollection ie = Interest.Children;
			setcheck(ae, attentiveness);
			setcheck(ee, effort);
			setcheck(ie, interest);
		}

		private void setcheck(UIElementCollection elements, int val)
		{
			foreach (RadioButton e in elements)
			{
				if (Int32.Parse(e.Content.ToString()) == val)
				{
					e.IsChecked = true;
				}
			}
		}


		public void writeToJSON(AOI aoi) //adapted from https://stackoverflow.com/questions/20626849/how-to-append-a-json-file-without-disturbing-the-formatting
		{
			if (aoi != null)
			{
				//set values
				aoi.timeRangeStart = DisplaySlider.LowerValue + startTime;
				aoi.timeRangeEnd = DisplaySlider.HigherValue + startTime;
				aoi.Interest = interest;
				aoi.Attentiveness = attentiveness;
				aoi.Effort = effort;

				aoi.convertPolygon();

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
					if (Equals(aoi.Name, a.Name) && Equals(aoi.test, a.test))
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

		public void exit()
		{
			endMarkup();
		}

		private void Aoi_CheckBox_Click(object sender, RoutedEventArgs e)
		{
			if ((bool)((CheckBox)e.Source).IsChecked)
			{
				_mainWindow.SelectionCanvas.Visibility = Visibility.Visible;
			} else
			{
				_mainWindow.SelectionCanvas.Visibility = Visibility.Hidden;
			}
		}
	}
}
