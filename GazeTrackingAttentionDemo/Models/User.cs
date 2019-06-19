using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GazeTrackingAttentionDemo.Models
{
	public class User
	{
		public List<Test> testList;
		public List<String> testPaths;
		public int highestTestIndex;

		public User(String id, String groupName, String groupPath)
		{
			DateTime dateAndTime = DateTime.Now;
			this.Id = id;
			this.GroupName = groupName;
			this.GroupPath = groupPath;
			this.DirPath = "C:\\MMAD\\Subjects\\" + id + "_" + groupName + "_" + dateAndTime.ToString("dd-MM-yyyy--HH-mm-ss");
			this.InfoPath = DirPath + "\\" + id + "Info.txt";
			DirectoryInfo di = Directory.CreateDirectory(DirPath);
			if (File.Exists(InfoPath)){
				File.WriteAllText(InfoPath, "");
			}
			File.AppendAllText(InfoPath, id + Environment.NewLine + groupName + Environment.NewLine);

			testPaths = getTestPaths();
			testList = new List<Test>();

			highestTestIndex = -1;
		}


		public List<String> getTestPaths()
		{
			//create sorted list of path strings
			List<String>filePaths = new List<String>(Directory.GetFiles(GroupPath));
			filePaths.Sort((String a, String b) =>
			{
				//sort files into order based on number in title
				int a_num = Int32.Parse(Regex.Match(Path.GetFileNameWithoutExtension(a), @"\d+").Value);
				int b_num = Int32.Parse(Regex.Match(Path.GetFileNameWithoutExtension(b), @"\d+").Value);

				return a_num.CompareTo(b_num);
			});
			return filePaths;
		}

		//public Test createTest(String testType)
		//{
		//	testIndex++;
		//	tests.Add(new Test(this, testType, testPaths[testIndex], testIndex));
		//	return tests[testIndex];
		//	//	currentTest = currentUser.tests[testIndex];
		//	//	State = EState.Ready;
		//}

		private String _id;
		private String _groupName;
		private String _groupPath;
		private String _dirPath;
		private String _infoPath;
		private Test _currentTest;

		public Test CurrentTest { get => _currentTest ; set => _currentTest = value; }
		public string Id { get => _id; set => _id = value; }
		public string GroupName { get => _groupName; set => _groupName = value; }
		public string GroupPath { get => _groupPath; set => _groupPath = value; }
		public string DirPath { get => _dirPath; set => _dirPath = value; }
		public string InfoPath { get => _infoPath; set => _infoPath = value; }
	}
}
