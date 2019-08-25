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
using GazeTrackingAttentionDemo.Models;
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
		public String UserGroup {get; set;}
		public String UserID { get; set; }

		public delegate void UserCreatedHandler(User newUser);
		public event UserCreatedHandler UserCreated;

		public delegate void DirectLoadHandler();
		public event DirectLoadHandler DirectLoad;

		public delegate void ExitHandler();
		public event ExitHandler ExitProgram;

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
			//DirectLoad += new DirectLoadHandler(mainWin.onDirectLoad);
			ExitProgram += new ExitHandler(mainWin.shutdown);
		}

		private void CreateUser_Click(object sender, RoutedEventArgs e)
		{
            User u;
			UserCreated(u = new User(UserID, UserGroup, ("C:\\MMAD\\TestGroups\\" + UserGroup)));
            Session.newSession(u);
            ObjectManager.saveUser(u);
		}

		private void Debug_Directload_Click(object sender, RoutedEventArgs e)
		{
			DirectLoad();
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			ExitProgram();
		}

        private void Markup_Click(object sender, RoutedEventArgs e)
        {
            mainWin.State = MainWindow.EState.Markup;
        }
    }
}
