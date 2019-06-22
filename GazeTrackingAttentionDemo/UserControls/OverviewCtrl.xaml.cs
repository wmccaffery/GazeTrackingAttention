using GazeTrackingAttentionDemo.Models;
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
//using System.Windows.Shapes;

namespace GazeTrackingAttentionDemo.UserControls
{
	/// <summary>
	/// Interaction logic for TestCreationCtrl.xaml
	/// </summary>
	public partial class OverviewCtrl : UserControl
	{
		public List<String> testPaths;
		public List<String> mediums;
		
		//public Test SelectedTest { get; set; }

		public delegate void TestLoadedHandler();
		public event TestLoadedHandler testLoaded;

		public delegate void MarkupHandler();
		public event MarkupHandler markupStarted;
			
		MainWindow mainWin = (MainWindow)Application.Current.MainWindow;
		User user;

		ControlWindow parentWin;


		public OverviewCtrl()
		{
			InitializeComponent();
			DataContext = this;

		}

		public void onLoad(object sender, RoutedEventArgs e)
		{
			parentWin = (ControlWindow)Window.GetWindow(this);
			user = mainWin.currentUser;
			testPaths = user.getTestPaths();
			if(user.testList.Count == 0)
			{
				genTests();
			}
			testList.ItemsSource = user.testList;
			int highestTest = user.highestTestIndex;
			if(highestTest < user.testList.Count - 1)
			{
				testList.SelectedItem = user.testList[user.highestTestIndex + 1];

			}
			mediums = new List<String> { "Select medium", "PAPER", "SCREEN" };
			stimuliMedium.ItemsSource = mediums;
			stimuliMedium.SelectedItem = "Select medium";
			testLoaded += new TestLoadedHandler(mainWin.testCreated);
			testLoaded += new TestLoadedHandler(parentWin.testCreated);
			markupStarted += new MarkupHandler(mainWin.allTestsCompleted);
			testList.Items.Refresh();
		}

		//fill list with test objects
		private void genTests()
		{
			int i = 0;
			foreach (String s in testPaths)
			{
				user.testList.Add(new Test(user, s, i));
				i++;
			}
		}


		//load selected test
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			String medium; 
			if((medium = stimuliMedium.SelectedItem.ToString()) != "Select medium")
			{
				Test test = (Test)testList.SelectedItem;
				test.dataRecorder = new DataProcessing.StreamReader();
				test.setMedium(medium);
				test.dataRecorder.initLSLProviders();
				test.dataRecorder.feedStreamsToLSL();
				test.dataRecorder.resolveAllStreams();
				user.CurrentTest = test;
				Console.WriteLine("Test Loaded!");
				
				testLoaded();
			}
		}

		private void TestList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Test t = (Test)((ListBox)e.Source).SelectedItem;
			nextBtn.Content = "Run Test " + Path.GetFileNameWithoutExtension(t.StimuliPath);
		}

		private void StartMarkup_Click(object sender, RoutedEventArgs e)
		{
			mainWin.State = MainWindow.EState.Markup;
		}

		Boolean eegrecord;
		DataProcessing.StreamReader eegDataRecorder;


		private void RecordEEG_Click(object sender, RoutedEventArgs e)
		{
			if (!eegrecord)
			{
				eegDataRecorder = new DataProcessing.StreamReader();
				eegDataRecorder.resolveEEGStream();
				eegDataRecorder.setRecordingPaths(true, user.DirPath + @"\");
				eegDataRecorder.readStreams();
				eegDataRecorder.recordStreams();
				((Button)e.Source).Content = "Stop Recording";
				startMarkup.IsEnabled = false;
				nextBtn.IsEnabled = false;
				stimuliMedium.IsEnabled = false;
				eegrecord = true;
			} else
			{
				eegDataRecorder.stopRecording();
				eegDataRecorder.closeLslEEGStream();
				eegDataRecorder.Dispose();
				((Button)e.Source).Content = "Record EEG";
				startMarkup.IsEnabled = true;
				nextBtn.IsEnabled = true;
				stimuliMedium.IsEnabled = true;
				eegrecord = false;

			}

		}
	}

}
