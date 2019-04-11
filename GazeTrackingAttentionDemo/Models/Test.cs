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
		public StreamReader fd;
		

		public Test(User user, String testPath, int testIndex)
		{
			TestPath = testPath;
			fd = new StreamReader();
			TestDir = user.DirPath + "//Test_" + testIndex;
			DirectoryInfo di = Directory.CreateDirectory(TestDir);

		}

		public string TestPath { get => _testPath; set => _testPath = value; }
		public string User { get => _user; set => _user = value; }
		public string TestDir { get => _testDir; set => _testDir = value; }

		public void testComplete()
		{
			fd.Dispose();
		}
	}
}
