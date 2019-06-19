using GazeTrackingAttentionDemo.DataVisualization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeTrackingAttentionDemo.Models
{
	public class Test
	{
		private string _testDataDir;	//path to store all data collected during the test
		private string _stimuliPath;	//path of stimuli for test
		private string _metaDataPath;	//path to store data on test
		private string _currentUser;    //string username of user taking test
		private string _testName;
		private string _recordingDir;
		private int _recordingNum;
		public DataProcessing.StreamReader dataRecorder;
		public Boolean isPaper;
		public String infoPath;
		public int index;
		public List<Recording> recordings;
		public Recording currentRecording;

		

		public Test(User user, String testPath, int testIndex)
		{
			DateTime dateTime = DateTime.Now;
			this.index = testIndex;
			StimuliPath = testPath;
			_testName = Path.GetFileNameWithoutExtension(_stimuliPath);
			Console.WriteLine(StimuliPath);

			//create new instance of data recorder
			dataRecorder = new DataProcessing.StreamReader();

			//create path for test
			TestDir = user.DirPath + "\\" + user.Id + "_test" + testIndex + "_" + Path.GetFileNameWithoutExtension(StimuliPath) + "_" + dateTime.ToString("dd-MM-yyyy");



			//create directory and meta file for test
			DirectoryInfo di;
			di = Directory.CreateDirectory(TestDir);

			recordings = new List<Recording>();

			_recordingNum = -1;

		}

		public void addNewRecording(User user)
		{
			DateTime dateTime = DateTime.Now;
			Console.WriteLine("TESTDIR " + TestDir);
			DataDir = "";
			DataDir = TestDir + "\\Recording_" + (++_recordingNum) + "_" + dateTime.ToString("HH-mm-ss");
			DirectoryInfo di = Directory.CreateDirectory(DataDir);
			InfoPath = DataDir + "\\meta.txt";
			currentRecording = new Recording(_recordingNum, isPaper, DataDir);
			recordings.Add(currentRecording);


			//append content
			File.AppendAllText(InfoPath, "Current User " + user.Id + Environment.NewLine);
			File.AppendAllText(InfoPath, "Current User Group " + user.GroupName + Environment.NewLine);
			File.AppendAllText(InfoPath, "Test Name " + Path.GetFileNameWithoutExtension(StimuliPath) + Environment.NewLine);
			File.AppendAllText(InfoPath, "Test Number " + index + Environment.NewLine);

			if (isPaper)
			{
				File.AppendAllText(InfoPath, "Test Medium: Paper" + Environment.NewLine);
			}
			else
			{
				File.AppendAllText(InfoPath, "Test Medium: Screen" + Environment.NewLine);
			}

			File.AppendAllText(InfoPath, "Original Test Path: " + StimuliPath + Environment.NewLine);
			File.AppendAllText(InfoPath, "Original Group Path: " + user.GroupPath + Environment.NewLine);


		}

		public void setMedium(String medium)
		{
			isPaper = (medium.Equals("PAPER"));
		}


		public string Name { get => _testName; set => _testName = value; }
		public string StimuliPath { get => _stimuliPath; set => _stimuliPath = value; }
		public string User { get => _currentUser; set => _currentUser = value; }
		public string TestDir { get => _testDataDir; set => _testDataDir = value; }
		public string DataDir { get => _recordingDir; set => _recordingDir = value; }
		public string InfoPath { get => _metaDataPath; set => _metaDataPath = value; }
		public int RecordingNum { get => _recordingNum; set => RecordingNum = value; }

		public void testComplete()
		{
			dataRecorder.Dispose();
		}
	}
}
