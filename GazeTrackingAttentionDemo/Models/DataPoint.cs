using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeTrackingAttentionDemo.Models
{
	public class DataPoint
	{
		public float x;
		public float y;
		public float timestamp;

		public DataPoint(float x, float y, float timestamp)
		{
			this.x = x;
			this.y = y;
			this.timestamp = timestamp;
		}

		public Point toPoint()
		{
			return new Point(x, y);
		}

		public string toString => "{" + x + "," + y + "}";
	}
}
