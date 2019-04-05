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
	/// Interaction logic for UserCtrl.xaml
	/// </summary>
	/// 
	public partial class UserCtrl : UserControl
	{

		public List<String> GroupPaths { get; set; }
		public List<String> GroupNames { get; set; }

		public delegate void UserCreatedHandler();

		public event UserCreatedHandler UserCreated;

		MainWindow mainWin = (MainWindow)Application.Current.MainWindow;
		ControlWindow parentWin;


		public UserCtrl()
		{
			InitializeComponent();
			this.DataContext = this;

			//discover groups;
			String groupDirPath = "C:\\MMAD\\TestGroups\\";
			GroupPaths = new List<String>(Directory.GetDirectories(groupDirPath));
			GroupNames = new List<String>();

			foreach (String s in GroupPaths)
			{
				String dirName = System.IO.Path.GetFileName(s);
				Console.WriteLine(dirName + " discovered");
				GroupNames.Add(dirName);
			}
			Console.WriteLine(GroupPaths.Count + " test groups were discovered");


		}

		public void onLoad(object sender, RoutedEventArgs e)
		{
			//parentWin = (Window)((ContentControl) this.Parent).Parent;
			parentWin = (ControlWindow)Window.GetWindow(this);
			//define event handler in main window
			UserCreated += new UserCreatedHandler(mainWin.onUserCreated);
			UserCreated += new UserCreatedHandler(parentWin.onUserCreated);

		}




		private void CreateUser_Click(object sender, RoutedEventArgs e)
		{
			UserCreated();

		}
	}
}
