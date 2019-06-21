using GazeTrackingAttentionDemo.DataVisualization;
using GazeTrackingAttentionDemo.Models;
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
	/// Interaction logic for SelectionCtrl.xaml
	/// </summary>
	public partial class AoiCtrl : UserControl
	{
		//public List<String> Paragraphs { get; set; }

		//public String SelectedParagraph { get; set; }


		public delegate void StartSelectionHandler();
		public event StartSelectionHandler StartSelection;

		public delegate void SelectedTestChangeHandler(Test test);
		public event SelectedTestChangeHandler selectedTestChanged;

		public delegate void SelectedRecordingChangeHandler(Recording test);
		public event SelectedRecordingChangeHandler selectedRecordingChanged;

		public delegate void SelectedAOIChangeHandler(AOI aoi);
		public event SelectedAOIChangeHandler selectedAOIChanged;



		private MainWindow _mainWin = (MainWindow)Application.Current.MainWindow;

		User user;
		Test _selectedTest;
		Recording _selectedRecording;

		public Test SelectedTest
		{
			get { return _selectedTest; }
			set { _selectedTest = value;
				selectedTestChanged(_selectedTest);
			}
		}

		public Recording SelectedRecording
		{
			get { return _selectedRecording; }
			set
			{
				_selectedRecording = value;
				selectedRecordingChanged(_selectedRecording);
			}
		}

		AOI _selectedAOI;
		AOI SelectedAOI
		{
			get { return _selectedAOI; }
			set { _selectedAOI = value;
				selectedAOIChanged(_selectedAOI);
			}
		}


		public AoiCtrl()
		{
			InitializeComponent(); 
			this.DataContext = this;

		}

		public void onLoad(object sender, RoutedEventArgs e)
		{
			user = _mainWin.currentUser;
			List<string> testPaths = user.getTestPaths();
			List<string> testNames = new List<string>();


			//foreach (string s in testPaths)
			//{
			//	testNames.Add(System.IO.Path.GetFileNameWithoutExtension(s));
			//}

			testList.ItemsSource = user.testList;

			StartSelection += new StartSelectionHandler(_mainWin.onAOICreation);
			selectedTestChanged += new SelectedTestChangeHandler(((MarkupCtrl)_mainWin.rightView.Content).onTestChanged);
			selectedRecordingChanged += new SelectedRecordingChangeHandler(((MarkupCtrl)_mainWin.rightView.Content).onRecordingChanged);
			selectedAOIChanged += new SelectedAOIChangeHandler(((MarkupCtrl)_mainWin.rightView.Content).onAOIChanged);

		}

		private void DrawAoi_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedRecording != null)
			{
				StartSelection();
			}
		}

		public void endSelection(Polygon p)
		{
			AOI aoi = new AOI(SelectedRecording.testName, SelectedRecording.user);
			aoi.p = p;
			aoi.Name = "AOI_" + SelectedRecording.Aois.Count;
			SelectedRecording.Aois.Add(aoi);
			aoiList.Items.Refresh();

			foreach (AOI a in SelectedRecording.Aois)
			{
				a.p.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 250, 205));
				a.p.Stroke = new SolidColorBrush(Color.FromRgb(255, 250, 205));
				_mainWin.SelectionCanvas.Children.Add(a.p);
			}
		}

		private void TestList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//clear canvas
			_mainWin.MainCanvas.Children.Clear();
			_mainWin.SelectionCanvas.Children.Clear();

			SelectedTest = (Test)((ListBox)e.Source).SelectedItem;

			//render text
			if (SelectedTest != null)
			{
				((DocumentCtrl)_mainWin.centerView.Content).loadText(user.GroupPath + "\\" + SelectedTest.Name + ".rtf");

				recordingList.ItemsSource = SelectedTest.recordings;
				recordingList.Items.Refresh();
			} else
			{
				recordingList.ItemsSource = null;
				recordingList.Items.Refresh();
			}

			aoiList.ItemsSource = null;
			aoiList.Items.Refresh();

		}

		//should change test listbox to use templates better like this
		private void AoiList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedAOI = (AOI)((ListBox)e.Source).SelectedItem;

			_mainWin.SelectionCanvas.Children.Clear();

			if (SelectedAOI != null)
			{
				foreach (AOI a in SelectedRecording.Aois)
				{
					if (Equals(a, SelectedAOI))
					{
						a.p.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 250, 205));
						a.p.Stroke = new SolidColorBrush(Color.FromRgb(255, 250, 205));

					}
					else
					{
						a.p.Fill = new SolidColorBrush(Color.FromArgb(100, 211, 211, 211));
						a.p.Stroke = new SolidColorBrush(Color.FromRgb(211, 211, 211));
					}
					_mainWin.SelectionCanvas.Children.Add(a.p);

				}
			}
		}

		private void RecordingList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//clear canvas
			_mainWin.MainCanvas.Children.Clear();
			_mainWin.SelectionCanvas.Children.Clear();

			//	//render text
			//	String selected = ((ListBox)e.Source).SelectedItem.ToString();
			//	((DocumentCtrl)_mainWin.centerView.Content).loadText(_mainWin.currentUser.GroupPath + "\\" + selected + ".rtf");
			SelectedRecording = (Recording)((ListBox)e.Source).SelectedItem;

			//render gaze plot

			if (SelectedRecording != null)
			{
				//update aoi list
				aoiList.ItemsSource = SelectedRecording.Aois;
				aoiList.Items.Refresh();

				//render areas of interest
				_mainWin.SelectionCanvas.Children.Clear();
				foreach (AOI a in SelectedRecording.Aois)
				{
					a.p.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 250, 205));
					a.p.Stroke = new SolidColorBrush(Color.FromRgb(255, 250, 205));
					_mainWin.SelectionCanvas.Children.Add(a.p);
				}

				//render gaze plot
				SelectedRecording.gp = new GazePlot(SelectedRecording.fixations, SelectedRecording.saccades, _mainWin.MainCanvas);
				SelectedRecording.gp.renderPlot(true, true, true, 0, 999999999);
			}
			//else
			//{
			//	//if for some reason test data is missing for a file provide an error message
			//	AOI dummyAOI = new AOI(SelectedRecording);
			//	dummyAOI.Name = "WARNING: No test data found for this file!";
			//	aoiList.ItemsSource = new List<AOI>() { dummyAOI };
			//	aoiList.Items.Refresh();
			//}
		}
	}
}

//old
//discover groups;
//String groupDirPath = "C:\\MMAD\\TestGroups\\";
//GroupPaths = new List<String>(Directory.GetDirectories(groupDirPath));
//GroupNames = new List<String>();
//Paragraphs = new List<string>();

//String currentTestPath = _mainWin.getCurrentTest(); //TODO this should really be done through an event watching for currentTest to change

//foreach (String s in GroupPaths)
//{
//	String dirName = System.IO.Path.GetFileName(s);
//	Console.WriteLine(dirName + " discovered");
//	GroupNames.Add(dirName);
//}
//Console.WriteLine(GroupPaths.Count + " test groups were discovered");

//}

//public void numParagraphsUpdated(int num)
//{
//	for(int i = 0; i < num; i++)
//	{
//		Paragraphs.Add("" + num);
//	}
//}


//public void onLoad(object sender, RoutedEventArgs e)
//{
//	//parentWin = (Window)((ContentControl) this.Parent).Parent;
//	//parentWin = (ControlWindow)Window.GetWindow(this);

//	//define event handler in main window
//	//UserCreated += new UserCreatedHandler(mainWin.onUserCreated);
//	//UserCreated += new UserCreatedHandler(parentWin.onUserCreated);
//	//_mainWin = (MainWindow)Application.Current.MainWindow;

//	//((UserControls.DocumentCtrl)_mainWin.centerView).numParagraphsFound += new DocumentCtrl.numParagraphsFoundHandler(this.numParagraphsUpdated);


//	//StartSelection += new StartSelectionHandler(_mainWin.startSelection);
//	//EndSelection += new EndSelectionHandler(_mainWin.endSelection);

//}

//parentWin = (ControlWindow)Window.GetWindow(this);

