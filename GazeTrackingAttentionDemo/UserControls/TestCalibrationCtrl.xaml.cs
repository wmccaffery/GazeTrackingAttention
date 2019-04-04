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
    /// Interaction logic for TestCalibrationCtrl.xaml
    /// </summary>
    public partial class TestCalibrationCtrl : UserControl
    {

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


		public double DocumentWidth { get => _documentWidth; }
		public double DocumentHeight { get => _documentHeight; }
		public double DocumentMarginWidth { get => _documentMarginWidth; }
		public double DocumentMarginHeight { get => _documentMarginHeight; }

		public TestCalibrationCtrl()
        {
            InitializeComponent();

			this.DataContext = this;
		}

		public void onLoad(object sender, RoutedEventArgs e)
		{
			var field = testArea;
			UIElement container = VisualTreeHelper.GetParent(field) as UIElement;
			Point viewerSource = field.TranslatePoint(new Point(0, 0), container);
			double elementHeight = field.ActualHeight;
			double elementWidth = field.ActualWidth;

			Point s1 = viewerSource;
			Point s2 = new Point(viewerSource.X, viewerSource.Y + elementHeight);
			Point s3 = new Point(viewerSource.X, viewerSource.Y + elementHeight / 2);
			Point s4 = new Point(viewerSource.X, viewerSource.Y + elementHeight / 4);
			Point s5 = new Point(viewerSource.X, viewerSource.Y + elementHeight / 2 + elementHeight / 4);

			Point s6 = new Point(viewerSource.X + elementWidth / 4, viewerSource.Y);
			Point s7 = new Point(viewerSource.X + elementWidth / 4, viewerSource.Y + elementHeight / 2);
			Point s8 = new Point(viewerSource.X + elementWidth / 4, viewerSource.Y + elementHeight / 4);
			Point s9 = new Point(viewerSource.X + elementWidth / 4, viewerSource.Y + elementHeight / 2 + elementHeight / 4);
			Point s10 = new Point(viewerSource.X + elementWidth / 4, viewerSource.Y + elementHeight);

			Point s11 = new Point(viewerSource.X + elementWidth / 2, viewerSource.Y);
			Point s12 = new Point(viewerSource.X + elementWidth / 2, viewerSource.Y + elementHeight / 2);
			Point s13 = new Point(viewerSource.X + elementWidth / 2, viewerSource.Y + elementHeight / 4);
			Point s14 = new Point(viewerSource.X + elementWidth / 2, viewerSource.Y + elementHeight / 2 + elementHeight / 4);
			Point s15 = new Point(viewerSource.X + elementWidth / 2, viewerSource.Y + elementHeight);

			Point s16 = new Point(viewerSource.X + elementWidth / 2 + elementWidth / 4, viewerSource.Y);
			Point s17 = new Point(viewerSource.X + elementWidth / 2 + elementWidth / 4, viewerSource.Y + elementHeight / 2);
			Point s18 = new Point(viewerSource.X + elementWidth / 2 + elementWidth / 4, viewerSource.Y + elementHeight / 4);
			Point s19 = new Point(viewerSource.X + elementWidth / 2 + elementWidth / 4, viewerSource.Y + elementHeight / 2 + elementHeight / 4);
			Point s20 = new Point(viewerSource.X + elementWidth / 2 + elementWidth / 4, viewerSource.Y + elementHeight);

			Point s21 = new Point(viewerSource.X + elementWidth, viewerSource.Y);
			Point s22 = new Point(viewerSource.X + elementWidth, viewerSource.Y + elementHeight / 2);
			Point s23 = new Point(viewerSource.X + elementWidth, viewerSource.Y + elementHeight / 4);
			Point s24 = new Point(viewerSource.X + elementWidth, viewerSource.Y + elementHeight / 2 + elementHeight / 4);
			Point s25 = new Point(viewerSource.X + elementWidth, viewerSource.Y + elementHeight);


			drawTestTarget(s1);
			drawTestTarget(s2);
			drawTestTarget(s3);
			drawTestTarget(s4);
			drawTestTarget(s5);
			drawTestTarget(s6);
			drawTestTarget(s7);
			drawTestTarget(s8);
			drawTestTarget(s9);
			drawTestTarget(s10);
			drawTestTarget(s11);
			drawTestTarget(s12);
			drawTestTarget(s13);
			drawTestTarget(s14);
			drawTestTarget(s15);
			drawTestTarget(s16);
			drawTestTarget(s17);
			drawTestTarget(s18);
			drawTestTarget(s19);
			drawTestTarget(s20);
			drawTestTarget(s21);
			drawTestTarget(s22);
			drawTestTarget(s23);
			drawTestTarget(s24);
			drawTestTarget(s25);
		}

		private void drawTestTarget(Point p)
		{
			//add number
			Ellipse r = new Ellipse();
			r.Height = 10;
			r.Width = 10;

			SolidColorBrush mySolidColorBrush = new SolidColorBrush(Colors.Black);

			r.Fill = mySolidColorBrush;

			//myEllipse.StrokeThickness = 2;

			Canvas.SetLeft(r, p.X - (r.Width / 2));
			Canvas.SetTop(r, p.Y - (r.Height / 2));

			mainCanvas.Children.Add(r);

			Ellipse r2 = new Ellipse();
			r2.Height = 50;
			r2.Width = 50;

			SolidColorBrush mySolidColorBrush2 = new SolidColorBrush(Colors.Transparent);

			r2.Fill = mySolidColorBrush2;

			r2.Stroke = Brushes.Black;

			Canvas.SetLeft(r2, p.X - (r2.Width / 2));
			Canvas.SetTop(r2, p.Y - (r2.Height / 2));

			mainCanvas.Children.Add(r2);
		}


	}
}

