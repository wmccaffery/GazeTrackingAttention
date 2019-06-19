using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace GazeTrackingAttentionDemo.Models
{
	public class AOI
	{
		public string location;
		string _name;
		int _interest;
		int _attentiveness;
		int _effort;
		String _comments;
		public Polygon p;
		//data

		public AOI(Recording parent)
		{
			this.location = parent.dataDir;
		}

		

		public string Name { get => _name; set => _name = value; }
		public int Interest { get => _interest; set => _interest = value; }
		public int Attentiveness { get => _attentiveness; set => _attentiveness = value; }
		public int Effort { get => _effort; set => _effort = value; }
		public string Comments { get => _comments; set => _comments = value; }



	}
}
