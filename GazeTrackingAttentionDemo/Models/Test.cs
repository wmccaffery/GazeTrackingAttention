using GazeTrackingAttentionDemo.DataVisualization;
using GazeTrackingAttentionDemo.DeviceInteraction;
using Newtonsoft.Json;
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
		private string _userID;    //string username of user taking test
		private string _testName;
		private string _recordingDir;
        private int _numRecordings;
		//private int _recordingNum;
        //[JsonIgnore]
		//public DeviceInteractionHost dataRecorder;
		public Boolean isPaper;
		//public String infoPath;
		public int index;
		//public List<Recording> recordings;
        //[JsonIgnore]
		//public Recording currentRecording;
		//public String testName;

		

		public Test(string userId, string userDir, String testPath, int testIndex)
		{
			DateTime dateTime = DateTime.Now;
			this.index = testIndex;
			StimuliPath = testPath;
			//testName = Path.GetFileNameWithoutExtension(StimuliPath) + Environment.NewLine;
			_testName = Path.GetFileNameWithoutExtension(_stimuliPath);
			_userID = userId;
			Console.WriteLine(StimuliPath);

			//create new instance of data recorder
			//dataRecorder = new DataProcessing.StreamReader(); this is done externally now
			
			//create path for test
			TestDir = userDir + "\\" + _userID + "_test" + testIndex + "_" + Path.GetFileNameWithoutExtension(StimuliPath) + "_" + dateTime.ToString("dd-MM-yyyy");


            //recordings = new List<Recording>();

            //_recordingNum = -1;
            _numRecordings = 0;

		}

		public void addNewRecording(User user)
		{
			DateTime dateTime = DateTime.Now;
			string dataDir = TestDir + "\\" + Path.GetFileName(TestDir) + "_recording" + (_numRecordings);
            DirectoryInfo di = Directory.CreateDirectory(dataDir);
            Session.currentRecording = new Recording(_numRecordings, isPaper, dataDir, Name, UserID);
            _numRecordings++;
            ObjectManager.saveSession();
		}

		public void setMedium(String medium)
		{
			isPaper = (medium.Equals("PAPER"));
		}


		public string Name { get => _testName; set => _testName = value; }
		public string StimuliPath { get => _stimuliPath; set => _stimuliPath = value; }
		public string UserID { get => _userID; set => _userID = value; }
		public string TestDir { get => _testDataDir; set => _testDataDir = value; }
		//public string DataDir { get => _recordingDir; set => _recordingDir = value; }
		public string InfoPath { get => _metaDataPath; set => _metaDataPath = value; }
        //public int RecordingNum { get => _recordingNum; set => RecordingNum = value; }
        //public int RecordingNumDisp { get => _recordingNum + 1; }
        public int numRecordings { get => _numRecordings; set => _numRecordings = value; }

        public void testComplete()
		{
			Session.dataRecorder.Dispose();
		}
	}
}
