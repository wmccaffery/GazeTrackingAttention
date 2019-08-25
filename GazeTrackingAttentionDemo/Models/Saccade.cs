using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace GazeTrackingAttentionDemo.Models
{
	public class Saccade
	{
		public Fixation start;
		public Fixation finish;
		public double duration;
		public double width;
		public double height;
		public double length;
		public int id;

		public void completeSaccade(int i)
		{
			id = i;
			duration = finish.startPos.timestamp - start.endPos.timestamp;
			width = finish.centroid.X - start.centroid.X;
			height = finish.centroid.Y - start.centroid.Y;
			length = Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2));
		}
	}
}