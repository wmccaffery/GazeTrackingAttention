using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace GazeTrackingAttentionDemo.Models
{
	public class AOI
	{
		public string user;
		public string test;
		string _name;
		int _interest;
		int _attentiveness;
		int _effort;
		double _startTime;
		double _endTime;
		public String[] points;

		[JsonIgnore]
		public Polygon p;



		public AOI(String testName, String user)
		{
			this.test = testName;
			this.user = user;
			this.Interest = 1;
			this.Attentiveness = 1;
			this.Effort = 1;
		}

		//convert polygon into array of points for json
		public void convertPolygon()
		{
			String[] points = new String[p.Points.Count];
			for(int i = 0; i < p.Points.Count; i++)
			{
				points[i] = "X " + p.Points[i].X + " Y " + p.Points[i]; 
			}
			this.points = points;
		}

		public string Name { get => _name; set => _name = value; }
		public int Interest { get => _interest; set => _interest = value; }
		public int Attentiveness { get => _attentiveness; set => _attentiveness = value; }
		public int Effort { get => _effort; set => _effort = value; }
		public double timeRangeStart { get => _startTime; set => _startTime = value; }
		public double timeRangeEnd { get => _endTime; set => _endTime = value; }





	}
}
