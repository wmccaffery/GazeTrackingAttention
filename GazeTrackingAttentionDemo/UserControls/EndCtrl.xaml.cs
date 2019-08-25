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
	/// Interaction logic for EndCtrl.xaml
	/// </summary>
	public partial class EndCtrl : UserControl
	{
		MainWindow _mainWin = (MainWindow)Application.Current.MainWindow;

		public delegate void endMarkupHandler();
		public event endMarkupHandler endMarkup;


		public delegate void exitHandler();
		public event exitHandler exitProgram;

		Boolean toOverview;
		Boolean exit;
        Boolean toSetup;

		public EndCtrl()
		{
			InitializeComponent();

		}

		public void onLoad(object sender, RoutedEventArgs e)
		{
			MarkupCtrl mc = (MarkupCtrl)_mainWin.rightView.Content;
			endMarkup += new endMarkupHandler(mc.exit);
			exitProgram += new exitHandler(_mainWin.shutdown);
		}

		public void markupExitComplete()
		{
			if (exit)
			{
				exitProgram();
			} else if (toOverview)
			{
				_mainWin.State = MainWindow.EState.Overview;
			} else if (toSetup)
            {
                _mainWin.State = MainWindow.EState.Setup;
            }
			exit = false;
			toOverview = false;
            toSetup = false;
		}

		private void End_Click(object sender, RoutedEventArgs e)
		{
			exit = true;
			endMarkup();
		}

		private void Back_Click(object sender, RoutedEventArgs e)
		{
            if (Session.currentUser == null)
            {
                toSetup = true;
                endMarkup();
            }
            else
            {
                toOverview = true;
                endMarkup();
            }
		}
	}
}
