using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeTrackingAttentionDemo.Models
{
	public class User
	{
		public User(String id, String groupName, String groupPath)
		{
			this.Id = id;
			this.GroupName = groupName;
			this.GroupPath = groupPath;
			this.DirPath = "C:\\MMAD\\Subjects\\Subject_" + id;
			this.InfoPath = DirPath + "\\Subject_" + id + "Info.txt";
			DirectoryInfo di = Directory.CreateDirectory(DirPath);
			if (File.Exists(InfoPath)){
				File.WriteAllText(InfoPath, "");
			}
			File.AppendAllText(InfoPath, id + Environment.NewLine + groupName + Environment.NewLine);
		}

		private String _id;
		private String _groupName;
		private String _groupPath;
		private String _dirPath;
		private String _infoPath;


		public string Id { get => _id; set => _id = value; }
		public string GroupName { get => _groupName; set => _groupName = value; }
		public string GroupPath { get => _groupPath; set => _groupPath = value; }
		public string DirPath { get => _dirPath; set => _dirPath = value; }
		public string InfoPath { get => _infoPath; set => _infoPath = value; }
	}
}
