﻿using GazeTrackingAttentionDemo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GazeTrackingAttentionDemo.UserControls
{
	/// <summary>
	/// Interaction logic for DocumentCtrl.xaml
	/// </summary>a
	public partial class DocumentCtrl : UserControl
	{


		//public delegate void numParagraphsFoundHandler(int numParagraphs);
		//public event numParagraphsFoundHandler numParagraphsFound;

		public int numParagraphs;

		public DocumentCtrl()
		{
			InitializeComponent();
			
			this.DataContext = this;

		}

		public void onLoad(object sender, RoutedEventArgs e)
		{
			PageText.Document.Blocks.Clear();
			//PageText.Selection.Load(new FileStream(@"C:\MMAD\TestGroups\GroupA\Test1.rtf", FileMode.Open), DataFormats.Rtf);
			//new font with correct
			//PageText.Document.Blocks.Add(new Paragraph(new Run("Ready to start test ")));
			Console.WriteLine("Document control Loaded");
		}

		//public void onTestLoaded(String test)
		//{
		//	clearText();
		//	PageText.Selection.Load(new FileStream(test, FileMode.Open), DataFormats.Rtf);
		//	PageText.FontSize = 17.85;
		//	PageText.FontFamily = new FontFamily("Calibri");

		//	int  numParagraphs = 0;
		//	foreach (var paragraph in PageText.Document.Paragraphs())
		//	{
		//		numParagraphs++;
		//	}
		//	numParagraphsFound(numParagraphs);
		//}

		//public void loadTestText(Test test)
		//{
		//	string testPath = test.TestPath;
		//	clearText();
		//	PageText.Selection.Load(new FileStream(testPath, FileMode.Open), DataFormats.Rtf);
		//	PageText.FontSize = 17.85;
		//	PageText.FontFamily = new FontFamily("Calibri");

		//	int numParagraphs = 0;
		//	foreach (var paragraph in PageText.Document.Paragraphs())
		//	{
		//		numParagraphs++;
		//	}
		//	numParagraphsFound(numParagraphs);
		//}

		public void loadText(String _fileName)
		{
			clearText();
			//FileStream fs = new FileStream(text, FileMode.Open);
			//PageText.Selection.Load(fs, DataFormats.Rtf);
			//fs.Close();
			TextRange range;
			FileStream fStream;
			if (File.Exists(_fileName))
			{
				range = new TextRange(PageText.Document.ContentStart, PageText.Document.ContentEnd);
				fStream = new FileStream(_fileName, FileMode.OpenOrCreate);
				range.Load(fStream, DataFormats.Rtf);
				fStream.Close();
			}
			PageText.FontSize = 17.85;
			PageText.FontFamily = new FontFamily("Calibri");

			numParagraphs = PageText.Document.Paragraphs().Count();
			Console.WriteLine("Loaded " + numParagraphs + " paragraphs");

		}


		public void clearText()
		{
			PageText.Document.Blocks.Clear();

		}

		//Calculate sizes to draw a full sized A4 page 

		private static double _a4Width = 8.27;
		private static double _a4Height = 11.69;
		private static double _a4MarginSize = 1;
		private static double _xScreenRes = 1920;
		private static double _yScreenRes = 1200;
		private static double _xScreenWidth = 20.4;
		private static double _yScreenHeight = 12.8;


		private double _documentWidth = (_a4Width * (_xScreenRes / _xScreenWidth));

			private double _documentHeight = _a4Height * (_yScreenRes / _yScreenHeight);

			private double _documentMarginWidth = _a4MarginSize * (_xScreenRes / _xScreenWidth) - 10;

			private double _documentMarginHeight = _a4MarginSize * (_xScreenRes / _xScreenWidth) - 10;


			public double DocumentWidth { get => _documentWidth;}
			public double DocumentHeight { get => _documentHeight;}
			public double DocumentMarginWidth { get => _documentMarginWidth;}
			public double DocumentMarginHeight { get => _documentMarginHeight;}

	}

	//CODE BLOCK TAKEN FROM https://stackoverflow.com/questions/27716601/c-sharp-read-paragraph-from-rich-text-box
	//Author DBC
	//Originally posted December 31st 2014
	//Edited January 3rd 2015
	//Accessed 30th May 2019

	public static class FlowDocumentExtensions
	{
		public static IEnumerable<Paragraph> Paragraphs(this FlowDocument doc)
		{
			return doc.Descendants().OfType<Paragraph>();
		}
	}

	public static class DependencyObjectExtensions
	{
		public static IEnumerable<DependencyObject> Descendants(this DependencyObject root)
		{
			if (root == null)
				yield break;
			yield return root;
			foreach (var child in LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>())
				foreach (var descendent in child.Descendants())
					yield return descendent;
		}
	}


	//END OF CODE BLOCK


	//private void RtbMouseMove(object sender, MouseEventArgs e)
	//{
	//    RichTextBox rtb = sender as RichTextBox;
	//    rtb.Focus();
	//    Point point = Mouse.GetPosition(rtb);

	//    TextPointer tp = rtb.GetPositionFromPoint(point, true);
	//    Paragraph p = tp.Paragraph;

	//    TextPointer start = p.ContentStart;
	//    TextPointer end = p.ContentEnd;

	//    rtb.Selection.Select(start, end);
	//}

	//private void RtbMouseDown(object sender, MouseButtonEventArgs e)
	//{
	//    Console.WriteLine("Triggered");
	//    RichTextBox rtb = sender as RichTextBox;
	//    TextPointer start = rtb.Selection.Start;
	//    Paragraph p = start.Paragraph;
	//    Console.WriteLine("paragraph " + p.Name + " selected");
	//}

	//private void render()
	//{
	//    StackTrace stackTrace = new StackTrace();
	//    Console.WriteLine("I was called by " + stackTrace.GetFrame(1).GetMethod().Name);
	//    mainCanvas.Children.Clear();
	//    //if (State.fixations.Count == 0)
	//    //{
	//    //    Console.WriteLine("nothing to render");
	//    //}
	//        if (test)
	//        {
	//            renderTestTargets();
	//        }

	//        if (showFixations)
	//        {
	//            //double durationSum = 0;
	//            for (int i = 0; i < session.currentTestResults.fixationData.Count; i++)
	//            {
	//                Fixation f = (Fixation)session.currentTestResults.fixationData[i];
	//                //if (f.startPoint.timeStamp <= displayTime)
	//                {
	//                Console.WriteLine("duration " + f.duration);
	//                renderEllipse(this.CenterSubGrid.PointFromScreen(f.centroid.toPoint()), Color.FromArgb(127, Colors.Blue.R, Colors.Blue.G, Colors.Blue.B), i,Math.Sqrt(f.duration/100/Math.PI));
	//                }
	//            }
	//        }

	//        if (showSaccades)
	//        {
	//            foreach (Saccade l in session.currentTestResults.SaccadeData)
	//            {
	//                {
	//                    //if (l.start <= displayTime)
	//                    renderLine(l, Colors.Black);
	//                }
	//            }
	//        }

	//        if (showGaze)
	//        {
	//            foreach (DataPoint p in session.currentTestResults.rawGazeData)
	//            {
	//            //if (p.timeStamp <= displayTime)
	//            //{
	//                    //renderEllipse(p.toPoint(), Colors.Red, 5);
	//                    Point pt = this.CenterSubGrid.PointFromScreen(p.toPoint());
	//                    renderEllipse(pt, Colors.Gold, 5);
	//                //}
	//            }
	//        }
	//    }


	//private void renderEllipse(Point p, Color c, int num, double radius)
	//{
	//    //draw ellipse
	//    Ellipse e = new Ellipse();

	//    SolidColorBrush mySolidColorBrush = new SolidColorBrush(c);

	//    e.Fill = mySolidColorBrush;

	//    //myEllipse.StrokeThickness = 2;

	//    e.Stroke = Brushes.Black;

	//    e.Width = radius;

	//    e.Height = radius;


	//    Canvas.SetLeft(e, p.X - (e.Width / 2));
	//    Canvas.SetTop(e, p.Y - (e.Height / 2));

	//    mainCanvas.Children.Add(e);

	//    //add number
	//    TextBlock t = new TextBlock();
	//    t.Text = "" + num;
	//    t.Foreground = new SolidColorBrush(Colors.White);
	//    t.Height = 25;
	//    t.Width = 25;
	//    t.TextAlignment = TextAlignment.Center;
	//    t.VerticalAlignment = VerticalAlignment.Center;
	//    t.FontSize = 20;
	//    t.FontWeight = FontWeights.Bold;
	//    //t.Background = new SolidColorBrush(Colors.Red);

	//    Canvas.SetLeft(t, p.X - (t.Width / 2));
	//    Canvas.SetTop(t, p.Y - (t.Height / 2));

	//    mainCanvas.Children.Add(t);
	//}

	//private void renderEllipse(Point p, Color c, double radius)
	//{
	//    //draw ellipse
	//    Ellipse e = new Ellipse();

	//    SolidColorBrush mySolidColorBrush = new SolidColorBrush(c);

	//    e.Fill = mySolidColorBrush;

	//    //myEllipse.StrokeThickness = 2;

	//    e.Stroke = Brushes.Black;

	//    e.Width = radius;

	//    e.Height = radius;

	//    Canvas.SetLeft(e, p.X - (e.Width / 2));
	//    Canvas.SetTop(e, p.Y - (e.Height / 2));

	//    mainCanvas.Children.Add(e);
	//}

	//private void renderLine(Saccade line, Color c)
	//{
	//    try
	//    {
	//        Point p1 = new Point();
	//        Point p2 = new Point();

	//        p1.X = line.X1;
	//        p1.Y = line.Y1;
	//        p2.X = line.X2;
	//        p2.Y = line.Y2;

	//        Point p1_fixed = this.CenterSubGrid.PointFromScreen(p1);
	//        Point p2_fixed = this.CenterSubGrid.PointFromScreen(p2);

	//        Line l = new Line();
	//        l.X1 = p1_fixed.X;
	//        l.X2 = p2_fixed.X;
	//        l.Y1 = p1_fixed.Y;
	//        l.Y2 = p2_fixed.Y;
	//        l.Stroke = new SolidColorBrush(c);
	//        l.StrokeThickness = 2;
	//        mainCanvas.Children.Add(l);
	//    }
	//    catch (Exception e)
	//    {
	//        //the bug that caused this should have been patched out
	//        Console.WriteLine("WARN: line failed to render");
	//    }
	//}


}
