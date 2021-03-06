﻿using GazeTrackingAttentionDemo.DeviceInteraction;
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
			user = Session.currentUser;
			testPaths = user.getTestPaths();
			if(Session.testList.Count == 0)
			{
				genTests();
			}
			testList.ItemsSource = Session.testList;
			int highestTest = user.highestTestIndex;
			if(highestTest < Session.testList.Count - 1)
			{
				testList.SelectedItem = Session.testList[user.highestTestIndex + 1];

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
                Test t;
				Session.testList.Add(t = new Test(Session.currentUser.Id, Session.currentUser.DirPath, s, i));
                Directory.CreateDirectory(t.TestDir);
                i++;
			}
		}


		//load selected test
		private void StartTest_Click(object sender, RoutedEventArgs e)
		{
			String medium; 
			if((medium = stimuliMedium.SelectedItem.ToString()) != "Select medium")
			{
				Test test = (Test)testList.SelectedItem;
                Session.dataRecorder = new DeviceInteractionHost();
                DeviceInteractionHost dataRecorder = Session.dataRecorder;
				dataRecorder.initLSLProviders();
				dataRecorder.feedStreamsToLSL();
				dataRecorder.RegisterLSLDevice(new LSLDevice("EEG", true));
				dataRecorder.RegisterLSLDevice(new LSLDevice("Gaze", false));
				dataRecorder.RegisterLSLDevice(new LSLDevice("Quality", false));
				//DEBUG
				//dataRecorder.feedDebugData();
				//dataRecorder.feedDebug2Data();
				//dataRecorder.RegisterLSLDevice(new LSLDevice("Debug", false));
				//dataRecorder.RegisterLSLDevice(new LSLDevice("Debug2", false));
				//DEBUG
				dataRecorder.resolveStreams();
				dataRecorder.createThreads();

				test.setMedium(medium);
				Session.currentTest = test;
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
		public DeviceInteraction.DeviceInteractionHost eegDataRecorder;

		private int baselinecount = 0;
		private void RecordEEG_Click(object sender, RoutedEventArgs e)
		{
			if (!eegrecord)
			{
				eegDataRecorder = new DeviceInteraction.DeviceInteractionHost();
				eegDataRecorder.RegisterLSLDevice(new LSLDevice("EEG", true));
				//DEBUG
				//eegDataRecorder.initLSLProviders();
				//eegDataRecorder.feedDebugData();
				//eegDataRecorder.RegisterLSLDevice(new LSLDevice("Debug", true));
				//DEBUG
				eegDataRecorder.resolveStreams();
				eegDataRecorder.setRecordingPaths(true, user.DirPath + @"\", baselinecount);
				eegDataRecorder.createThreads();
				eegDataRecorder.startStreaming();
				eegDataRecorder.startRecording();
				((Button)e.Source).Content = "Stop Recording";
				startMarkup.IsEnabled = false;
				nextBtn.IsEnabled = false;
				stimuliMedium.IsEnabled = false;
				eegrecord = true;
			} else
			{
				eegDataRecorder.stopRecording();
				eegDataRecorder.stopStreaming();
				eegDataRecorder.Dispose();
				((Button)e.Source).Content = "Record EEG";
				startMarkup.IsEnabled = true;
				nextBtn.IsEnabled = true;
				stimuliMedium.IsEnabled = true;
				eegrecord = false;
				baselinecount++;

			}

		}
	}

}
