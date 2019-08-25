
using GazeTrackingAttentionDemo.DataVisualization;
using GazeTrackingAttentionDemo.DeviceInteraction;
using GazeTrackingAttentionDemo.Models;
using GazeTrackingAttentionDemo.UserControls;
using LSL;
using Microsoft.Expression.Encoder.Devices;
using System;
using System.Collections;    
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Tobii.Interaction;
using Tobii.Interaction.Framework;
using Application = System.Windows.Application;
using UserControl = System.Windows.Controls.UserControl;

namespace GazeTrackingAttentionDemo
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public enum EState { Wait, Setup, Test, ReadyToCalibrate, Ready, Streaming, Recording, DoneRecording, Selection, ActiveSelection, Markup }
        //Wait: time between tests
        //Setup: user creation and initial calibration
        //Test: testing user calibration
        //ReadyToRecord: ready to start recording data
        //Recording: data is being recorded
        //DoneRecording: data has been recorded
        //Selection: loaded interface for selecting datapoints for paragraphs
        //ActiveSelection: in the process of selecting datapoints
        //Markup: data is being annotated


        //user controls
        UserControl overview;
        UserControl stimuliDisplayArea;
        UserControl selectionCtrl;
        UserControl markupCtrl;
        UserControl calibrationTest;
        UserControl calibrationCtrl;

        //second window
        public ControlWindow ctrlwin;

        public Stopwatch stopwatch = new Stopwatch();
        public Int32 unixMSStartTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

        //program state
        public enum EState { Setup, Overview, Ready, Calibrating, Streaming, Recording, Drawing, Markup }
        private EState _state;

        bool drawingAOI;

        public Ellipse PointOfOrigin;
        int ellipseCount = 0;
        Polygon aoi;

        //event handlers
        //public delegate void LoadTestHandler(String test);
        //public event LoadTestHandler loadTest;

        public delegate void readyToRecordHandler(Test test);
        public event readyToRecordHandler readyToRecord;

        public delegate void recordingStartHandler();
        public event recordingStartHandler recordingStarted;

        public delegate void recordingEndHandler();
        public event recordingEndHandler recordingStopped;

        public delegate void stateChangedHandler(EState state);
        public event stateChangedHandler progStateChanged;

        public delegate void selectionCompleteHandler(Polygon aoi);
        public event selectionCompleteHandler aoiSelectionComplete;


        //collection of all video devices available
        public Collection<EncoderDevice> VideoDevices { get; set; }

        //list of all file paths
        public List<String> filePaths { get; set; }

        public EState State
        {
            get { return _state; }
            set
            {
                _state = value;
                progStateChanged(_state);
            }
        }

        //currently unused vars
        //init gaze stream
        //StreamReader fd;
        //Thread eegStream;
        //Session session;
        //public Collection<EncoderDevice> AudioDevices { get; set; }
        //public delegate void testChangedHandler(String path);
        //public event testChangedHandler currentTestChanged;
        ////current test
        //private Test _currentTestInstance = null;
        //public Test currentTestInstance
        //{
        //	get { return _currentTestInstance; }
        //	set
        //	{
        //		if(_currentTestInstance != null) //add previous value to list
        //		{
        //			testList.Add(_currentTestInstance);
        //		}
        //		_currentTestInstance = value;
        //	}
        //}

        public MainWindow()
        {
            stopwatch.Start();

            //set window to start on the second monitor
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            Screen s2 = Screen.AllScreens[0];
            //Screen s2 = Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();
            System.Drawing.Rectangle r2 = s2.WorkingArea;
            this.Top = r2.Top;
            this.Left = r2.Left;
            this.Width = r2.Width;
            this.Height = r2.Height;

            //set datacontext and init window loading
            this.DataContext = this;
            InitializeComponent();

            //find available video devices for webcam
            VideoDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);

            //Create child window
            SourceInitialized += (s, a) =>
            {
                ctrlwin = new ControlWindow();
                ctrlwin.Owner = this;
                ctrlwin.Show();
            };

            //loadTest += new LoadTestHandler(((UserControls.DocumentCtrl)stimuliDisplayArea).onTestLoaded);

            progStateChanged += new stateChangedHandler(stateChanged);


            //readyToRecord += new readyToRecordHandler(ctrlwin.readyToRecord);

            //recordingStarted += new recordingStartHandler(ctrlwin.startRecording);

            //recordingStopped += new recordingEndHandler(ctrlwin.stopRecording);
        }


        public void onLoad(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            Application.Current.MainWindow = this;
            State = EState.Setup;

            overview = new UserControls.OverviewCtrl();
            stimuliDisplayArea = new UserControls.DocumentCtrl();
            selectionCtrl = new AoiCtrl();
            markupCtrl = new MarkupCtrl();
            calibrationTest = new UserControls.TestCalibrationCtrl();
            calibrationCtrl = new UserControls.CalibrationCtrl();

            aoiSelectionComplete += new selectionCompleteHandler(((AoiCtrl)selectionCtrl).endSelection);
            progStateChanged += new stateChangedHandler(ctrlwin.stateChanged);
            //this.KeyDown += new System.Windows.Input.KeyEventHandler(MainWindow_KeyDown);
        }

        public void stateChanged(EState state)
        {
            switch (state)
            {
                case EState.Overview:
                    centerView.Content = stimuliDisplayArea;
                    leftView.Content = null;
                    rightView.Content = null;
                    MainCanvas.Children.Clear();
                    SelectionCanvas.Children.Clear();
                    if (Equals(centerView.Content.GetType(), stimuliDisplayArea.GetType()))
                    {
                        ((DocumentCtrl)centerView.Content).clearText();
                    }

                    break;
                case EState.Streaming:
                    break;
                case EState.Markup:
                    rightView.Content = markupCtrl;
                    leftView.Content = selectionCtrl;
                    centerView.Content = stimuliDisplayArea;
                    ((DocumentCtrl)stimuliDisplayArea).clearText();
                    ((AoiCtrl)selectionCtrl).aoiList.SelectedItem = null;
                    ((AoiCtrl)selectionCtrl).recordingList.SelectedItem = null;
                    ((AoiCtrl)selectionCtrl).testList.SelectedItem = null;

                    break;
                case EState.Calibrating:
                    leftView.Content = null;
                    rightView.Content = null;
                    centerView.Content = calibrationCtrl;
                    break;
                case EState.Ready:
                    if (!Equals(ctrlwin.controller.Content.GetType(), ctrlwin.testCtrl.GetType()))
                    {
                        ctrlwin.controller.Content = ctrlwin.testCtrl;
                        centerView.Content = stimuliDisplayArea;
                    }
                    break;
                case EState.Setup:
                    leftView.Content = null;
                    rightView.Content = null;
                    centerView.Content = null;
                    break;
            }
        }


        //handle user being created in ControlWindow
        public void onUserCreated(User user)
        {

            //currentUser = user;
            ////State = EState.ReadyToCalibrate;


            //filePaths = currentUser.getTestPaths();
            State = EState.Overview;
        }

        public void testCreated()
        {
            State = EState.Ready;
        }

        public void startStreaming()
        {
            ((DocumentCtrl)centerView.Content).loadText(Session.currentTest.StimuliPath);
            State = EState.Streaming;
        }

        public void stopStreaming()
        {
            State = EState.Ready;
        }

        public void testCompleted()
        {
            //currentTest.testComplete();
            State = EState.Overview;
        }

        public void allTestsCompleted()
        {
            State = EState.Markup;
        }

        public void startCalibration()
        {
            Session.dataRecorder.calibrate();
            State = EState.Calibrating;
        }

        public void endCalibration()
        {
            State = EState.Ready;
        }

        public void onAOICreation()
        {
            PointOfOrigin = null;
            ellipseCount = 0;
            drawingAOI = true;
            aoi = new Polygon();
        }



        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (drawingAOI)
            {

                if (Type.Equals(e.OriginalSource, PointOfOrigin))
                {
                    drawingAOI = false;
                    SelectionCanvas.Children.Clear();
                    aoiSelectionComplete(aoi);
                }
                else
                {
                    Point mp = Mouse.GetPosition(this);
                    aoi.Points.Add(mp);

                    Ellipse ellipse = new Ellipse();
                    ellipse.Fill = Brushes.Yellow;
                    ellipse.Stroke = Brushes.Black;
                    ellipse.Width = ellipse.Height = 25;

                    //set ellipse position
                    Canvas.SetLeft(ellipse, mp.X - (ellipse.Width / 2));
                    Canvas.SetTop(ellipse, mp.Y - (ellipse.Height / 2));


                    if (ellipseCount == 0)
                    {
                        ellipse.Stroke = Brushes.Red;
                        PointOfOrigin = ellipse;
                    }

                    ellipseCount++;
                    SelectionCanvas.Children.Add(ellipse);
                }

                Console.WriteLine("OriginalSource " + e.OriginalSource.GetType());
            }
        }

        public void shutdown()
        {
            Application.Current.Shutdown();
        }

        //public void onDirectLoad()
        //{
        //	currentUser = new User("DummyUser", "Group1", "C:\\MMAD\\TestGroups\\Group1");
        //	currentUser.testList = new List<Test>();
        //	//gen tests
        //	int j = 0;
        //	List<String> testPaths = currentUser.getTestPaths();

        //	foreach (String s in testPaths)
        //	{
        //		currentUser.testList.Add(new Test(currentUser.Id, currentUser.DirPath, s, j));
        //		j++;
        //	}

        //	Test t = currentUser.testList[0];
        //	t.setMedium("SCREEN");
        //	t.dataRecorder = new DeviceInteraction.DeviceInteractionHost();
        //	//currentUser.testList.Add(t);
        //	String dataPath = t.TestDir + "Recording_0";

        //	currentUser.testList[0].recordings = new List<Recording>();
        //	currentUser.testList[0].addNewRecording(currentUser);
        //	currentUser.testList[0].currentRecording.fixations = new List<Fixation>();
        //	currentUser.testList[0].currentRecording.saccades = new List<Saccade>();
        //	currentUser.testList[0].currentRecording.videoQpcStartTime = 29355;
        //	currentUser.testList[0].currentRecording.videoQpcEndTime = 35130;




        //	//read from very old log file
        //	string videopath = @"C:\MMAD\DummyData\postTermUser21_Group_TestGroup_30-05-2019--09-15-17\postTermUser21_test0rtf30-05-2019--09-15-22\SubjectpostTermUser21QPCstart29355end35130.wmv";
        //	File.Copy(videopath, (currentUser.testList[0].currentRecording.dataDir + "\\SubjectpostTermUser21QPCstart29355end35130.wmv"));
        //	string filepath = @"C:\MMAD\DummyData\postTermUser21_Group_TestGroup_30-05-2019--09-15-17\postTermUser21_test0rtf30-05-2019--09-15-22\_test_030-05-2019--09-15-40_U1559204140_EYETRACKER_cleanFixationData.csv";
        //	StreamReader file = new StreamReader(filepath);
        //	string line;
        //	Fixation f = new Fixation();
        //	f.dataPos = new List<DataPoint>();
        //	int linenum = 0;

        //	//read in file
        //	int i = 0;
        //	while ((line = file.ReadLine()) != null)
        //	{
        //		if (linenum != 0)
        //		{
        //			string[] values = line.Split(',');
        //			String type = values[0];
        //			float x = float.Parse(values[1], System.Globalization.CultureInfo.InvariantCulture);
        //			float y = float.Parse(values[2], System.Globalization.CultureInfo.InvariantCulture);
        //			float timestamp = float.Parse(values[3], System.Globalization.CultureInfo.InvariantCulture);


        //			if (type.Equals("Begin"))
        //			{
        //				f.startPos = new DataPoint(x, y, timestamp);
        //			}
        //			else if (type.Equals("Data"))
        //			{
        //				f.dataPos.Add(new DataPoint(x, y, timestamp));
        //			}
        //			else
        //			{
        //				f.endPos = new DataPoint(x, y, timestamp);
        //				f.completeFixation(i);
        //				currentUser.testList[0].currentRecording.fixations.Add(f);
        //				f = new Fixation();
        //				i++;
        //				f.dataPos = new List<DataPoint>();
        //			}
        //		}
        //		linenum++;
        //	}
        //	file.Close();

        //	currentUser.testList[0].currentRecording.saccades = currentUser.testList[0].dataRecorder.getSaccades(currentUser.testList[0].currentRecording.fixations);

        //	State = EState.Markup;
        //}
    }
}
