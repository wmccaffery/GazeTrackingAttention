using System;
using System.Collections.Generic;
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
		}

		private String _id;
		private String _groupName;
		private String _groupPath;

		public string Id { get => _id; set => _id = value; }
		public string GroupName { get => _groupName; set => _groupName = value; }
		public string GroupPath { get => _groupPath; set => _groupPath = value; }
	}
}
