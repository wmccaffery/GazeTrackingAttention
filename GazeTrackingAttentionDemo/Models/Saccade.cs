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

		public Saccade()
		{
			
		}

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


//public struct Saccade
//{
//    public double X1;
//    public double X2;
//    public double Y1;
//    public double Y2;
//    public double start;
//    public double end;
//    public double hlength;
//    public double vlength;
//    public double size;
//    public double duration;

//    //public Boolean isRegression;

//    //public Saccade(double X1, double Y1, double X2, double Y2, double timestamp)
//    //{
//    //    this.X1 = X1;
//    //    this.X2 = X2;
//    //    this.Y1 = Y1;
//    //    this.Y2 = Y2;
//    //    this.timeStamp = timestamp;
//    //    hlength = X2 - X1;
//    //    vlength = Y2 - Y1;
//    //    length = Math.Sqrt(Math.Pow(hlength, 2) + Math.Pow(vlength, 2));
//    //    //isRegression = (vlength < 0);
//    //}


//    //public Saccade(Point p1, Point p2, double timestamp)
//    //{
//    //    X1 = p1.X;
//    //    X2 = p2.X;
//    //    Y1 = p1.Y;
//    //    Y2 = p2.Y;
//    //    this.start = timestamp;
//    //    hlength = X2 - X1;
//    //    vlength = Y2 - Y1;
//    //    length = Math.Sqrt(Math.Pow(hlength, 2) + Math.Pow(vlength, 2));
//    //    //isRegression = (vlength < 0);
//    //}
//}
