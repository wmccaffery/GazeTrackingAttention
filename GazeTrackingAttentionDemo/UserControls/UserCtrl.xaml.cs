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
	public partial class UserCtrl : UserControl
	{
		public UserCtrl()
		{
			InitializeComponent();
			this.DataContext = this;
		
			//discover groups;
			String groupDirPath = "C:\\MMAD\\TestGroups\\";
			List<String> groupPaths = new List<String>(Directory.GetDirectories(groupDirPath));
			List<String> groupNames = new List<String>();

			foreach(String s in groupPaths)
			{ 
				String dirName = System.IO.Path.GetDirectoryName(s);
				Console.WriteLine(s + " discovered");
			}
			Console.WriteLine(groupPaths.Count + " test groups were discovered");
		}

	}
}
