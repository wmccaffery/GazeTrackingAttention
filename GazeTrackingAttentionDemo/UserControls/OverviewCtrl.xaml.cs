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

		public delegate void TestCreatedHandler(Test test);
		public event TestCreatedHandler testCreated;

		public delegate void AllTestsCompletedHandler();
		public event AllTestsCompletedHandler allTestsCompleted;
			
		MainWindow mainWin = (MainWindow)Application.Current.MainWindow;
		ControlWindow parentWin;

		Boolean finishedAllTests;

		public OverviewCtrl()
		{
			InitializeComponent();
			DataContext = this;
		}

		public void onLoad(object sender, RoutedEventArgs e)
		{
			parentWin = (ControlWindow)Window.GetWindow(this);
			testPaths = parentWin.user.getTestPaths();
			testList.ItemsSource = testPaths;
			mediums = new List<String> { "Select medium", "PAPER", "SCREEN" };
			stimuliMedium.ItemsSource = mediums;
			stimuliMedium.SelectedItem = "Select medium";
			testCreated += new TestCreatedHandler(mainWin.testCreated);
			testCreated += new TestCreatedHandler(parentWin.testCreated);
			allTestsCompleted += new AllTestsCompletedHandler(mainWin.allTestsCompleted);



		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			String selection; 

			if (finishedAllTests)
			{
				allTestsCompleted();
			}
			else if((selection = stimuliMedium.SelectedItem.ToString()) != "Select medium")
			{
				Test test = parentWin.user.createTest(selection);
				testCreated(test);


				if (test.index + 1 == parentWin.totalNumTests)
				{
					nextBtn.Content = "Go To Markup";
					finishedAllTests = true;
				}
				else
				{
					nextBtn.Content = "Start Test " + Path.GetFileNameWithoutExtension(testPaths[test.index + 1]);
				}
			}
		}
	}

}
