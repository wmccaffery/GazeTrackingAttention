using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace GazeTrackingAttentionDemo.UserControls
{
	/// <summary>
	/// Interaction logic for DocumentCtrl.xaml
	/// </summary>a
	public partial class DocumentCtrl : UserControl
	{
		public DocumentCtrl()
		{
			InitializeComponent();
			
			this.DataContext = this;

		}

		public void onLoad(object sender, RoutedEventArgs e)
		{
			PageText.Document.Blocks.Clear();
			PageText.Selection.Load(new FileStream(@"C:\MMAD\Test.rtf", FileMode.Open), DataFormats.Rtf);
		}


		//Calculate sizes to draw a full sized A4 page 

		private static double _a4Width = 8.27;
			private static double _a4Height = 11.69;
			private static double _a4MarginSize = 1;
			private static double _xScreenRes = 1920;
			private static double _yScreenRes = 1200;
			private static double _xScreenDPI = 20.4;
			private static double _yScreenDPI = 12.8;

			private double _documentWidth = (_a4Width * (_xScreenRes / _xScreenDPI));

			private double _documentHeight = _a4Height * (_yScreenRes / _yScreenDPI);

			private double _documentMarginWidth = _a4MarginSize * (_xScreenRes / _xScreenDPI) - 10;

			private double _documentMarginHeight = _a4MarginSize * (_xScreenRes / _xScreenDPI) - 10;


			public double DocumentWidth { get => _documentWidth;}
			public double DocumentHeight { get => _documentHeight;}
			public double DocumentMarginWidth { get => _documentMarginWidth;}
			public double DocumentMarginHeight { get => _documentMarginHeight;}
	}


}
