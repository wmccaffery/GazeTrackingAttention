using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GazeTrackingAttentionDemo.Models
{
	public class Fixation
	{
		public DataPoint startPos;
		public DataPoint endPos;
		public List<DataPoint> dataPos;

		public Point centroid;
		public double duration;

		public Fixation(DataPoint startPos, DataPoint endPos, List<DataPoint> dataPos)
		{
			this.startPos = startPos;
			this.endPos = endPos;
			this.dataPos = dataPos;
			completeFixation();
		}

		public Fixation()
		{

		}

		public void completeFixation()
		{
			centroid = getCentroid();
			duration = endPos.timestamp - startPos.timestamp;
		}

		private Point getCentroid() {
			float xsum = startPos.x + endPos.x;
			float ysum = startPos.y + endPos.y;
			float numPoints = dataPos.Count + 2; //+2 since total number of points includes start and end points

			foreach(DataPoint p in dataPos){
				xsum += p.x;
				ysum += p.y;
			}

			return new Point((xsum / numPoints), (ysum / numPoints));
		}
	}
}

//legacy fixation code

//public class Fixation
//{
//    //Point point1;
//    //Point point2;

//    public DataPoint startPoint;
//    public ArrayList points;
//    public DataPoint endPoint;
//    //public ArrayList lines;
//    public DataPoint centroid;

//    public double duration;

//    //move draw function inside here
//    //public Fixation()
//    //{
//    //    points = new ArrayList();
//    //    //lines = new ArrayList();
//    //}

//    //public void completeFixation()
//    //{
//    //    //if (points.Count > 0)
//    //    //{
//    //    //TODO implement time weights

//    //    ////calculate average duration of points (apart from end point)
//    //    //startPoint.duration = ((DataPoint)points[0]).timeStamp - startPoint.timeStamp;
//    //    //double pointDurationSum = startPoint.duration;
//    //    //for (int i = 1; i < points.Count; i++)
//    //    //{
//    //    //    DataPoint p = ((DataPoint)points[i]);
//    //    //    DataPoint prev = ((DataPoint)points[i - 1]);
//    //    //    p.duration = p.timeStamp - prev.timeStamp;
//    //    //    pointDurationSum += p.duration;
//    //    //}
//    //    //double avgPointDuration = pointDurationSum / (points.Count + 1);

//    //    ////calculate percentage difference from average for each point (apart from end point)
//    //    //startPoint.weight = (startPoint.duration - avgPointDuration) / 100;

//    //    //for (int i = 0; i < points.Count; i++)
//    //    //{
//    //    //    DataPoint p = ((DataPoint)points[i]);
//    //    //    p.weight += (p.duration - avgPointDuration) / 100;
//    //    //}

//    //    //get average x and y coordinates


//    //    double xsum = 0;
//    //    double ysum = 0;
//    //    int numPoints = 0;

//    //    xsum += startPoint.rawX;
//    //    ysum += startPoint.rawY;
//    //    numPoints++;


//    //    for (int i = 0; i < points.Count; i++)
//    //    {
//    //        DataPoint p = ((DataPoint)points[i]);
//    //        xsum += p.rawX;
//    //        ysum += p.rawY;
//    //        numPoints++;
//    //    }

//    //    xsum += endPoint.rawX;
//    //    ysum += endPoint.rawY;
//    //    numPoints++;


//    //    centroid = new DataPoint((xsum / numPoints), (ysum / numPoints));

//    //        //double cx = xsum / points.Count + 2;
//    //        //double cy = ysum / points.Count + 2;
//    //        //centroid = new DataPoint(cx, cy);W
//    //    }
//    ////}

//    //public void getLines()
//    //{
//    //check for existing fixations
//    //if (State.fixations.Count > 1)
//    //{
//    //    point1 = ((Fixation)State.fixations[State.fixations.Count-2]).endPoint;

//    //    point2 = startPoint;
//    //    lines.Add(new LineWithTimestamp(point1, point2, point2.timeStamp));
//    //}

//    //point1 = startPoint;

//    //foreach(Point p in points)
//    //{
//    //    point2 = p;
//    //    lines.Add(new LineWithTimestamp(point1, point2, point2.timeStamp));

//    //    point1 = p;
//    //}

//    //point2 = endPoint;
//    //lines.Add(new LineWithTimestamp(point1, point2, point2.timeStamp));
//    //}


//    //public DataPoint getCentroid()
//    //{
//    //    double totalX = 0;
//    //    double totalY = 0;
//    //    int count = 0;

//    //    totalX += startPoint.rawX;
//    //    totalY += startPoint.rawY;
//    //    count++;

//    //    foreach (DataPoint p in points)
//    //    {
//    //        totalX += p.rawX;
//    //        totalY += p.rawY;
//    //        count++;
//    //    }

//    //    totalX += endPoint.rawX;
//    //    totalY += endPoint.rawY;
//    //    count++;

//    //    return new DataPoint(totalX / count, totalY / count);
//    //}
//}

//public struct DataPoint
//{
//    public double rawX;
//    public double rawY;
//    public double timeStamp;
//    public double duration;

//    public DataPoint(double X, double Y)
//    {
//        this.rawX = X;
//        this.rawY = Y;
//        this.timeStamp = -1;
//        this.duration = -1;
//    }

//    public DataPoint(double X, double Y, double timeStamp)
//    {
//        this.rawX = X;
//        this.rawY = Y;
//        this.timeStamp = timeStamp;
//        this.duration = -1;
//    }

//    public Point toPoint()
//    {
//        return new Point(this.rawX, this.rawY);
//    }

//    public Point toClientPoint()
//    {
//        return new Point(0,0);
//    }

//    public String toString()
//    {
//        return "{" + rawX + "," + rawY + "}";
//    }
//}