using GazeTrackingAttentionDemo.DataVisualization;
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

		public delegate void AOICreatedHandler(AOI aoi);
		public event AOICreatedHandler aoiCreated;

		public delegate void AOIRemovedHandler();
		public event AOIRemovedHandler aoiRemoved;



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

		List<String> _paragraphNames;
		List<String> ParagraphNames
		{
			get { return _paragraphNames; }
			set { _paragraphNames = value; }
		}

		public AoiCtrl()
		{
			InitializeComponent(); 
			this.DataContext = this;

		}

		public void onLoad(object sender, RoutedEventArgs e)
		{
			user = Session.currentUser;
            //testList.ItemsSource = user.testList;
            userList.ItemsSource = Directory.GetDirectories(@"C:\MMAD\Subjects");

			StartSelection += new StartSelectionHandler(_mainWin.onAOICreation);
			selectedTestChanged += new SelectedTestChangeHandler(((MarkupCtrl)_mainWin.rightView.Content).onTestChanged);
			selectedRecordingChanged += new SelectedRecordingChangeHandler(((MarkupCtrl)_mainWin.rightView.Content).onRecordingChanged);
			selectedAOIChanged += new SelectedAOIChangeHandler(((MarkupCtrl)_mainWin.rightView.Content).onAOIChanged);
			aoiCreated += new AOICreatedHandler(((MarkupCtrl)_mainWin.rightView.Content).onAOICreated);
			aoiRemoved += new AOIRemovedHandler(((MarkupCtrl)_mainWin.rightView.Content).onAOIRemoved);
		}

		private void DrawAoi_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedRecording != null)
			{
				StartSelection();
				drawAoi.IsEnabled = false;
				removeAOI.IsEnabled = false;
			}
		}

		public void endSelection(Polygon p)
		{
			//create new aoi
			AOI aoi = new AOI(SelectedRecording.testName, SelectedRecording.userID);
			aoi.Name = paragraphBox.SelectedItem.ToString();
			aoi.p = p;

			//check if aoi for this paragraph already exists
			bool nameExists = false;
			int existingIndex = 0;
			foreach (AOI a in SelectedRecording.Aois)
			{
				if (Equals(a.Name, aoi.Name))
				{
					nameExists = true;
					existingIndex = SelectedRecording.Aois.IndexOf(a);
					break;
				}
			}
			if (!nameExists)
			{
				SelectedRecording.Aois.Add(aoi);
			} else
			{
				SelectedRecording.Aois[existingIndex] = aoi; 
			}


			foreach (AOI a in SelectedRecording.Aois)
			{
				a.p.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 250, 205));
				a.p.Stroke = new SolidColorBrush(Color.FromRgb(255, 250, 205));
				_mainWin.SelectionCanvas.Children.Add(a.p);
			}
			drawAoi.IsEnabled = true;
			removeAOI.IsEnabled = true;

			aoiCreated(aoi);

			aoiList.SelectedItem = aoi;
			aoiList.Items.Refresh();


		}

		private void TestList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//clear canvas
			_mainWin.MainCanvas.Children.Clear();
			_mainWin.SelectionCanvas.Children.Clear();

			SelectedTest = (Test)((ListBox)e.Source).SelectedItem;

			//losf text
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

			//foreach (var block in ((DocumentCtrl)_mainWin.centerView.Content).PageText.Document.Blocks)
			//{
			//	Console.WriteLine(new TextRange(block.ContentStart, block.ContentEnd).Text);
			//}
			paragraphBox.IsEnabled = true;
			paragraphBox.SelectedItem = null;
			drawAoi.IsEnabled = false;

			ParagraphNames = new List<String>();
			int numParagraphs = ((DocumentCtrl)_mainWin.centerView.Content).numParagraphs;
			for (int i = 0; i < numParagraphs; i++)
			{
				ParagraphNames.Add("Paragraph " + i);
			}
			paragraphBox.ItemsSource = ParagraphNames;
			paragraphBox.Items.Refresh();

			drawAoi.IsEnabled = false;


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
			removeAOI.IsEnabled = true;
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
				renderAOIS();

				//render gaze plot
				SelectedRecording.gp = new GazePlot(SelectedRecording.fixations, SelectedRecording.saccades, _mainWin.MainCanvas);
				SelectedRecording.gp.renderPlot(true, true, true, 0, 999999999);
			}
		}

		private void renderAOIS()
		{
			_mainWin.SelectionCanvas.Children.Clear();
			foreach (AOI a in SelectedRecording.Aois)
			{
				a.p.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 250, 205));
				a.p.Stroke = new SolidColorBrush(Color.FromRgb(255, 250, 205));
				_mainWin.SelectionCanvas.Children.Add(a.p);
				//renderAOIS();
			}
		}

		private void RemoveAOI_Click(object sender, RoutedEventArgs e)
		{

			AOI aoitoremove = (AOI)aoiList.SelectedItem;
			aoiList.SelectedItem = null;
			aoiRemoved();
			removeFromJSON(aoitoremove);
			SelectedRecording.Aois.Remove(aoitoremove);
			aoiList.Items.Refresh();
			renderAOIS();
			removeAOI.IsEnabled = false;
		}

		public void removeFromJSON(AOI aoi) //adapted from https://stackoverflow.com/questions/20626849/how-to-append-a-json-file-without-disturbing-the-formatting
		{
			if (aoi != null)
			{
				//set values
				string filePath = _selectedRecording.dataDir + "\\annotations.json";
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

				// remove aoi
				AOI aoitoremove = null;
				foreach(AOI a in aoiList) //since deserialized object wont be the same object
				{
					if(Equals(a.Name, aoi.Name))
					{
						aoitoremove = a;
					}
				}
				aoiList.Remove(aoitoremove);

				// Update json data string
				jsonData = JsonConvert.SerializeObject(aoiList);

				System.IO.File.WriteAllText(filePath, jsonData);
			}
		}

		private void ParagraphBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			drawAoi.IsEnabled = true;
		}

        private void UserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string userPath = ((string)userList.SelectedItem) + @"\userData.json";
            user = ObjectManager.loadUser(userPath);
            testList.ItemsSource = Session.testList;
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

