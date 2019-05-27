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
		private String _testPath;
		private String _testDir;
		private String _user;
		private String _infoPath;
		public StreamReader fd;
		public Boolean isPaper;
		public String infoPath;
		public int index;
		

		public Test(User user, String testPath, int testIndex)
		{
			DateTime dateTime = DateTime.Now;
			this.index = testIndex;
			TestPath = testPath;
			Console.WriteLine(TestPath);
			fd = new StreamReader();
			TestDir = user.DirPath + "\\" + user.Id + "_test" + testIndex + testPath.Substring(testPath.Length - 3) + dateTime.ToString("dd-MM-yyyy--HH-mm-ss");
			InfoPath = TestDir + "\\meta.txt";

			//TODO: this needs to be changed
			DirectoryInfo di = Directory.CreateDirectory(TestDir);
			if (File.Exists(InfoPath))
			{
				File.WriteAllText(InfoPath, "");
			}
			File.AppendAllText(InfoPath, "ISPAPER=FALSE");

		}

		public string TestPath { get => _testPath; set => _testPath = value; }
		public string User { get => _user; set => _user = value; }
		public string TestDir { get => _testDir; set => _testDir = value; }
		public string InfoPath { get => _infoPath; set => _infoPath = value; }

		public void testComplete()
		{
			fd.Dispose();
		}
	}
}
