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
		private int _recordingNum;
		public DataProcessing.StreamReader dataRecorder;
		public Boolean isPaper;
		public Boolean isCalibrated;
		public String infoPath;
		public int index;
		public List<Fixation> fixations;
		public GazePlot gp;
		public List<Saccade> saccades;

		List<AOI> _aois;
		public List<AOI> Aois { get => _aois; set => _aois = value; }
		

		public Test(User user, String medium, String testPath, int testIndex)
		{
			DateTime dateTime = DateTime.Now;
			this.index = testIndex;
			TestPath = testPath;
			Console.WriteLine(TestPath);
			isPaper = (medium.Equals("PAPER"));

			//create new instance of data recorder
			dataRecorder = new DataProcessing.StreamReader();

			//create path for test
			TestDir = user.DirPath + "\\" + user.Id + "_test" + testIndex + "_" + Path.GetFileNameWithoutExtension(TestPath) + "_" + dateTime.ToString("dd-MM-yyyy--HH-mm-ss");
			InfoPath = TestDir + "\\meta.txt";



			//create directory and meta file for test
			DirectoryInfo di;
			di = Directory.CreateDirectory(TestDir);

			//erase existing file
			if (File.Exists(InfoPath))
			{
				File.WriteAllText(InfoPath, "");
			}

			//append content
			File.AppendAllText(InfoPath, "Current User " + user.Id);
			File.AppendAllText(InfoPath, "Current User Group " + user.GroupName);
			File.AppendAllText(InfoPath, "Test Name " + Path.GetFileNameWithoutExtension(TestPath));
			File.AppendAllText(InfoPath, "Test Number " + testIndex);

			if (isPaper)
			{
				File.AppendAllText(InfoPath, "Test Medium: Paper");
			} else
			{
				File.AppendAllText(InfoPath, "Test Medium: Screen");
			}

			File.AppendAllText(InfoPath, "Original Test Path: " + TestPath);
			File.AppendAllText(InfoPath, "Original Group Path: " + user.GroupPath);


			//create directory for recording
			//RecordingNum = 0;
			//DataDir = TestDir + "Recording_" + RecordingNum;
			//di = Directory.CreateDirectory(DataDir);


			//hold completed fixations
			fixations = new List<Fixation>();

			//hold areas of interest
			Aois = new List<AOI>();
		}

		public void IncRecordingNum()
		{

			DataDir = TestDir + "Recording_" + (++_recordingNum);
			DirectoryInfo di = Directory.CreateDirectory(DataDir);
		}



		public string TestPath { get => _stimuliPath; set => _stimuliPath = value; }
		public string User { get => _currentUser; set => _currentUser = value; }
		public string TestDir { get => _testDataDir; set => _testDataDir = value; }
		public string DataDir { get => _testDataDir; set => _testDataDir = value; }
		public string InfoPath { get => _metaDataPath; set => _metaDataPath = value; }

		public void testComplete()
		{
			dataRecorder.Dispose();
		}
	}
}
