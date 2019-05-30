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
	public partial class SelectionCtrl : UserControl
	{
		//public List<String> GroupPaths { get; set; }
		public List<String> Paragraphs { get; set; }

		public String SelectedParagraph { get; set; }

		//public List<String> GroupNames { get; set; }
		//public String UserGroup { get; set; }
		//public String UserID { get; set; }

		public delegate void StartSelectionHandler(String paragraph);
		public delegate void EndSelectionHandler(String paragraph);

		public event StartSelectionHandler StartSelection;
		public event EndSelectionHandler EndSelection;

		private MainWindow _mainWin = (MainWindow)Application.Current.MainWindow;

		private string currentTestPath;

		public SelectionCtrl()
		{
			InitializeComponent();
			this.DataContext = this;

			//discover groups;
			//String groupDirPath = "C:\\MMAD\\TestGroups\\";
			//GroupPaths = new List<String>(Directory.GetDirectories(groupDirPath));
			//GroupNames = new List<String>();
			Paragraphs = new List<string>();

			String currentTestPath = _mainWin.getCurrentTest(); //TODO this should really be done through an event watching for currentTest to change

			//foreach (String s in GroupPaths)
			//{
			//	String dirName = System.IO.Path.GetFileName(s);
			//	Console.WriteLine(dirName + " discovered");
			//	GroupNames.Add(dirName);
			//}
			//Console.WriteLine(GroupPaths.Count + " test groups were discovered");
		}

		public void numParagraphsUpdated(int num)
		{
			for(int i = 0; i < num; i++)
			{
				Paragraphs.Add("" + num);
			}
		}

		public void onLoad(object sender, RoutedEventArgs e)
		{
			//parentWin = (Window)((ContentControl) this.Parent).Parent;
			//parentWin = (ControlWindow)Window.GetWindow(this);

			//define event handler in main window
			//UserCreated += new UserCreatedHandler(mainWin.onUserCreated);
			//UserCreated += new UserCreatedHandler(parentWin.onUserCreated);
			StartSelection += new StartSelectionHandler(_mainWin.startSelection);
			EndSelection += new EndSelectionHandler(_mainWin.endSelection);

		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			StartSelection(SelectedParagraph);
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			EndSelection(SelectedParagraph);
		}
	}
}
