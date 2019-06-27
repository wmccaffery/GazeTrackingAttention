using GazeTrackingAttentionDemo.LSLInteraction;
using LSL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using Tobii.Interaction;
using Tobii.Interaction.Framework;
using GazeTrackingAttentionDemo.Models;

namespace GazeTrackingAttentionDemo.DataProcessing
{
	public class DataRecorder : IDisposable
	{
		//read directly from device
		private readonly Host _host;
		private readonly FixationDataStream _fixationDataStream;
		private readonly GazePointDataStream _gazePointDataStream;


		//read from LSL
		private readonly LSLStreamInteractionHost _lslHost;
		private readonly LSLFixationDataStream _lslFixationDataStream;
		private readonly LSLGazeDataStream _lslGazeDataStream;
		private readonly LSLEEGDataStream _lslEEGDataStream;


		private MainWindow _mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;

		private Boolean _record;

		public List<Tuple<string, DataPoint>> rawFixationPoints = new List<Tuple<string, DataPoint>>();

		//LSL output streams
		public liblsl.StreamInfo gazeDataInfo;
		public liblsl.StreamOutlet gazeDataOutlet;

		public liblsl.StreamInfo fixationBeginInfo;
		public liblsl.StreamOutlet fixationBeginOutlet;

		public liblsl.StreamInfo fixationDataInfo;
		public liblsl.StreamOutlet fixationDataOutlet;

		public liblsl.StreamInfo fixationEndInfo;
		public liblsl.StreamOutlet fixationEndOutlet;

		//filepaths
		string metaData;
		string fixationRawPath;
		string fixationCleanPath;
		string gazeRawPath;
		string eegRawPath;



		public DataRecorder()
		{
			MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;
			//Device
			_host = new Host();

			//init tobii streams
			_fixationDataStream = _host.Streams.CreateFixationDataStream();
			//_fixationDataStream = _host.Streams.CreateFixationDataStream(Tobii.Interaction.Framework.FixationDataMode.Slow);
			_gazePointDataStream = _host.Streams.CreateGazePointDataStream();

			//init lslinteractionhost
			_lslHost = new LSLStreamInteractionHost();


			//init lsl streams, without outlets and inlets
			_lslFixationDataStream = _lslHost.CreateNewLslFixationDataStream();
			_lslGazeDataStream = _lslHost.CreateNewLslGazeDataStream();
			_lslEEGDataStream = _lslHost.CreateNewLslEEGDataStream();
		}



		public bool resolveAllStreams()
		{
			return resolveFixationStream() && resolveGazeStream() && resolveEEGStream();
		}

		public bool resolveFixationStream()
		{
			bool resolved;
			if (!(resolved = _lslGazeDataStream.tryResolveStreams()))
			{
				Console.WriteLine("WARNING: Fixation Stream not found");
			}
			return resolved; 
		}

		public bool resolveGazeStream()
		{
			bool resolved;
			if (!(resolved = _lslFixationDataStream.tryResolveStreams()))
			{
				Console.WriteLine("WARNING: Gaze Stream not found");
			}
			return resolved;
		}

		public bool resolveEEGStream()
		{
			bool resolved;
			if (!(resolved = _lslEEGDataStream.tryResolveStreams()))
			{
				Console.WriteLine("WARNING: EEG Stream not found");
			}
			return resolved;
		}


		public void initLSLProviders()
		{
			//provider
			gazeDataInfo = new liblsl.StreamInfo("GazeData", "Gaze", 3, 70, liblsl.channel_format_t.cf_double64, "tobiieyex");
			gazeDataOutlet = new liblsl.StreamOutlet(gazeDataInfo);

			fixationBeginInfo = new liblsl.StreamInfo("FixationBegin", "Gaze", 3, 70, liblsl.channel_format_t.cf_double64, "tobiieyex");
			fixationBeginOutlet = new liblsl.StreamOutlet(fixationBeginInfo);

			fixationDataInfo = new liblsl.StreamInfo("FixationData", "Gaze", 3, 70, liblsl.channel_format_t.cf_double64, "tobiieyex");
			fixationDataOutlet = new liblsl.StreamOutlet(fixationDataInfo);

			fixationEndInfo = new liblsl.StreamInfo("FixationEnd", "Gaze", 3, 70, liblsl.channel_format_t.cf_double64, "tobiieyex");
			fixationEndOutlet = new liblsl.StreamOutlet(fixationEndInfo);

			Console.WriteLine("LSL providers intialized");


		}

		//send data to LSL
		public void sendGazeToLSL(liblsl.StreamOutlet outlet, double x, double y, double ts)
		{
			double[] data = { x, y, ts };
			outlet.push_sample(data);
		}

		//calibrate eye tracker
		public void calibrate()
		{
			_host.Context.LaunchConfigurationTool(Tobii.Interaction.Framework.ConfigurationTool.Recalibrate, (data) => { });


			//this code was attempting to trigger an event when calibration was complete

			//EngineStateProvider engineStateProvider = new EngineStateProvider(_host.Context);

			//EngineStateObserver<EyeTrackingDeviceStatus> _engineStateObserverTrackerState = engineStateProvider.CreateEyeTrackingDeviceStatusObserver();

			//_engineStateObserverTrackerState.Changed += _mainWindow.onDeviceCalibration;

			Console.WriteLine("Calibrating eye tracker");
		}

		//read from gaze streams in parallel
		public void feedStreamsToLSL()
		{
			Console.WriteLine("reading streams");

			Thread gazeBeginStream = new Thread(() => streamGazeData());
			Thread fixationBeginStream = new Thread(() => streamFixationData());

			fixationBeginStream.Start();
			gazeBeginStream.Start();
		}

		public void streamGazeData()
		{
			_gazePointDataStream.GazePoint((x, y, timestamp) =>
			{
				sendGazeToLSL(gazeDataOutlet, x, y, timestamp);
			});
		}

		public void streamFixationData()
		{
			_fixationDataStream
				.Begin((x, y, timestamp) =>
				{
					sendGazeToLSL(fixationBeginOutlet, x, y, timestamp);
				})
				 .Data((x, y, timestamp) =>
				 {
					 sendGazeToLSL(fixationDataOutlet, x, y, timestamp);

				 })
				.End((x, y, timestamp) =>
				{
					sendGazeToLSL(fixationEndOutlet, x, y, timestamp);
				});
		}

		public void readStreams()
		{
			if (_lslFixationDataStream.eyeTrackerPresent) {
				Console.WriteLine("reading streams");
				_lslFixationDataStream
					.Begin((x, y, tobiits, timestamp) =>
					{
						timestamp = timestamp * 1000;
						timestamp += _lslHost.offset;
						Console.WriteLine("Fixation Begin\tX {0}\tY {1}\ttimestamp {2}", x, y, timestamp);
						string header = "X,Y,DeviceTimestamp, LSLTimestamp, AdjustedUnix" + Environment.NewLine;
						double[] data = { x, y, tobiits };

						if (_record)
						{
							recordStream(fixationRawPath, "Fixation", "Begin", header, data, timestamp);
						}
					})
					.Data((x, y, tobiits, timestamp) =>
					{
						timestamp = timestamp * 1000;
						timestamp += _lslHost.offset;
						Console.WriteLine("Fixation Data\tX {0}\tY {1}\ttimestamp {2}", x, y, timestamp);
						string header = "X,Y,DeviceTimestamp, LSLTimestamp, AdjustedUnix" + Environment.NewLine;
						double[] data = { x, y, tobiits };

						if (_record)
						{
							recordStream(fixationRawPath, "Fixation", "Data", header, data, timestamp);
						}

					})
					.End((x, y, tobiits, timestamp) =>
					{
						timestamp = timestamp * 1000;
						timestamp += _lslHost.offset;
						Console.WriteLine("Fixation End\tX {0}\tY {1}\ttimestamp {2}", x, y, timestamp);
						string header = "X,Y,DeviceTimestamp, LSLTimestamp, AdjustedUnix" + Environment.NewLine;
						double[] data = { x, y, tobiits };

						if (_record)
						{
							recordStream(fixationRawPath, "Fixation", "End", header, data, timestamp);
						}
					});

				_lslGazeDataStream.GazeData((x, y, tobiits, timestamp) =>
				{
					timestamp = timestamp * 1000;
					timestamp += _lslHost.offset;
					Console.WriteLine("Gaze\tX {0}\tY {1}\ttimestamp {2}", x, y, timestamp);
					string header = "X,Y,DeviceTimestamp, LSLTimestamp, AdjustedUnix" + Environment.NewLine;
					double[] data = { x, y, tobiits };

					if (_record)
					{
						recordStream(gazeRawPath, "Gaze", "", header, data, timestamp);
					}
				});
			}

			if (_lslEEGDataStream.eegPresent)
			{
				_lslEEGDataStream.EEGData((c0, c1, c2, c3, c4, c5, c6, c7, timestamp) =>
				{
					Console.WriteLine("EEG\tCHANNEL1 {0}\tCHANNEL2 {1}\tCHANNEL3 {2}\tCHANNEL4 {3}\tCHANNEL5 {4}\tCHANNEL6 {5}\tCHANNEL7 {6}\tCHANNEL8 {7}\ttimestamp {2}", c0, c1, c2, c3, c4, c5, c6, c7, timestamp);
					string header = "C1,C2,C3,C4,C5,C6,C7,C8,Timestamp, AdjustedUnix" + Environment.NewLine;
					double[] data = { c0, c1, c2, c3, c4, c5, c6, c7 };

					if (_record)
					{
						recordStream(eegRawPath, "EEG", "", header, data, timestamp);
					}
				});
			}

		}
		public void recordStream(String rawPath, String streamType, String dataType, String fileHeader, double[] data, double timestamp)
		{
			if (_record)
			{
				if (!File.Exists(rawPath))
				{
					File.WriteAllText(rawPath, "");
					File.WriteAllText(rawPath, fileHeader);
				}

				string datastr = "";

				if(streamType == "Fixation")
				{
					//append data into a single csv line
					//of the form "Begin," + x + "," + y + "," + timestamp + Environment.NewLine;

					datastr += dataType + ",";
					foreach(double d in data)
					{
						datastr += (d + ",");
					}
					datastr += (timestamp + "," + (TimeSpan.FromMilliseconds(timestamp).Seconds + _mainWindow.unixStartTime) +  Environment.NewLine);

					//store in program for cleaning and assigning
					rawFixationPoints.Add(Tuple.Create(dataType, new DataPoint((float)data[0], (float)data[1], (float)timestamp)));
				}
				else
				{
					foreach (double d in data)
					{
						datastr += (d + ",");
					}
					datastr += (timestamp + "," + (TimeSpan.FromMilliseconds(timestamp).Seconds + _mainWindow.unixStartTime) +  Environment.NewLine);
				}

				//write csv line to file
				File.AppendAllText(rawPath, datastr);

			}
		}

		//set recording directories
		public void setRecordingPaths(bool customFile, string customFilePath="")
		{
			//current user
			User user = _mainWindow.currentUser;
			Test test = user.CurrentTest;


			DateTime time = DateTime.Now;
			Int32 unixts = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

			if (customFile)
			{
				metaData = customFilePath + "//EEG_Baseline_" + user.Id + time.ToString("dd-MM-yyyy--HH-mm-ss") + "_U" + unixts;
			}
			else
			{
				string testDir = test.currentRecording.dataDir;
				int testNum = test.index;
				metaData = testDir + "//" + user.Id + "_test_" + testNum + time.ToString("dd-MM-yyyy--HH-mm-ss") + "_U" + unixts;
			}

			//create test paths
			fixationRawPath = metaData + "_EYETRACKER_rawFixationData.csv ";
			fixationCleanPath = metaData + "_EYETRACKER_cleanFixationData.csv";
			gazeRawPath = metaData + "_EYETRACKER_rawGazeData.csv";
			eegRawPath = metaData + "_EEG_rawEEGData.csv";
		}

		public string getCleanFixationPath()
		{
			return fixationCleanPath;
		}

		public void recordFixations(String rawPath, Fixation fixation)
		{
			if (!File.Exists(rawPath))
				{
					File.WriteAllText(rawPath, "");
					File.WriteAllText(rawPath, "Stream,X,Y,Timestamp" + Environment.NewLine);
				}

				File.AppendAllText(rawPath, "Begin," + fixation.startPos.x + "," + fixation.startPos.y + "," + fixation.startPos.timestamp + Environment.NewLine);
				foreach(DataPoint d in fixation.dataPos)
				{
					File.AppendAllText(rawPath, "Data," + d.x + "," + d.y + "," + d.timestamp + Environment.NewLine);
				}
				File.AppendAllText(rawPath, "End," + fixation.endPos.x + "," + fixation.endPos.y + "," + fixation.endPos.timestamp + Environment.NewLine);
		}

		public List<Fixation> getFixations()
		{
			Boolean fixationStart = false;

			List<Fixation> validFixations = new List<Fixation>();

			//valid fixation candidate
			Fixation f = new Fixation();

			//build valid fixations
			int i = 0;
			foreach(Tuple<string, DataPoint> t in rawFixationPoints)
			{
				if (fixationStart && isValid(t.Item2)) { //continue fixation
					if (t.Item1 == "Begin") //discard malformed fixation and start a new fixation
					{
						f = new Fixation();
						f.startPos = t.Item2;
						f.dataPos = new List<DataPoint>();

					}
					else if (t.Item1 == "End") //complete valid fixation and add to list
					{
						fixationStart = false;
						f.endPos = t.Item2;
						f.completeFixation(i);
						validFixations.Add(f);
						f = new Fixation();
						i++;
					}
					else //add data to fixation
					{
						f.dataPos.Add(t.Item2);
					}
					
				}
				else if(t.Item1 == "Begin" && isValid(t.Item2)) //start new fixation
				{
					fixationStart = true;
					f.startPos = t.Item2;
					f.dataPos = new List<DataPoint>();
				}
				else if (!isValid(t.Item2))  //discard malformed fixation start a new fixation but don't record a value since current is invalid
				{ 
					f = new Fixation();
					f.dataPos = new List<DataPoint>();
				} 
			}
			return validFixations;
		}

		public List<Saccade> getSaccades(List<Fixation> fixations)
		{
			List<Saccade> saccades = new List<Saccade>();
			Saccade s = new Saccade();
			int i = 0;
			foreach(Fixation f in fixations)
			{
				if(s.start == null)
				{
					s.start = f;
				}
				else
				{
					s.finish = f;
					s.completeSaccade(i);
					saccades.Add(s);
					s = new Saccade();
					s.start = f;
					i++;
				}
			}
			return saccades;
		}

		public bool isValid(DataPoint val)
		{
			return (!float.IsNaN(val.x) && !float.IsNaN(val.y));
		}

		public void recordStreams()
		{
			_record = true;
		}

		public void stopRecording()
		{
			_record = false;
		}

		public void closeAllLslStreams()
		{
			Console.WriteLine("Closing lsl streams...");
			_lslHost.closeStream(_lslFixationDataStream);
			_lslHost.closeStream(_lslGazeDataStream);
			_lslHost.closeStream(_lslEEGDataStream);
			Console.WriteLine("Done");

		}

		public void closeLslEEGStream()
		{
			Console.WriteLine("Closing lsl streams...");
			_lslHost.closeStream(_lslEEGDataStream);
			Console.WriteLine("Done");

		}


		//read fixations from eye tracker stream
		//      public void readFixationStream()
		//      {
		//	store fixation

		//	Fixation f = new Fixation();
		//	long startTime = -1;
		//	long endTime = -1;

		//	TODO send streams to LSL and perform cleaning of fixation data after receieving back from LSL
		//	_fixationDataStream
		//              .Begin((x, y, timestamp) =>
		//          {
		//		f.startPoint = new DataPoint(x, y, timestamp);
		//		Console.WriteLine("Fixation started at X:{0} Y:{1} Device timestamp: {2}", x, y, timestamp);
		//		startTime = Stopwatch.GetTimestamp();
		//	})
		//               .Data((x, y, timestamp) =>
		//          {
		//		f.points.Add(new DataPoint(x, y, timestamp));
		//		Console.WriteLine("During fixation at X:{0} Y:{1} Device timestamp: {2}", x, y, timestamp);

		//	})
		//              .End((x, y, timestamp) =>
		//          {
		//		endTime = Stopwatch.GetTimestamp();
		//		f.endPoint = new DataPoint(x, y, timestamp);
		//		if ((Double.IsNaN(f.endPoint.rawX) && Double.IsNaN(f.endPoint.rawY)) || startTime == -1)
		//		{
		//			Console.WriteLine("NOT A VALID FIXATION, DISCARDED");
		//		}
		//		else
		//		{
		//			Console.WriteLine("Fixation started at " + f.startPoint.timeStamp);
		//			Console.WriteLine("Fixation finished at X:{0} Y:{1} Device timestamp: {2}", x, y, timestamp);
		//			session.currentTestResults.fixationData.Add(f);
		//		}
		//		f.duration = endTime - startTime;
		//		f.completeFixation();
		//		findSaccade();
		//		f = new Fixation();
		//	});

		//	all this block of code does is print out the current fixation to thte console
		//          if (!Double.IsNaN(f.startPoint.rawX) && !Double.IsNaN(f.startPoint.rawY) && !Double.IsNaN(f.endPoint.rawX) && !Double.IsNaN(f.endPoint.rawY))
		//          {
		//              f.completeFixation();
		//              //Console.WriteLine(f.startPoint.X);
		//              //Console.WriteLine("duration {0})", f.duration);
		//              if (f.startPoint.timeStamp != 0)
		//              {
		//                  session.currentTestResults.fixationData.Add(f);
		//                  Console.WriteLine("NEW FIXATION: \n" +
		//                  "xPos:{0}\n" +
		//                  "yPos:{1}\n" +
		//                  "timestamp:{2}\n" +
		//                  "duration:{3}\n" +
		//                  "startPoint:{4}\n" +
		//                  "subPoints:{5}\n" +
		//                  "endPoint:{6}\n", f.centroid.rawX, f.centroid.rawY, f.startPoint.timeStamp, f.duration, f.startPoint.toString(), f.points.ToString(), f.endPoint.toString());
		//                  //attemptSaccade();
		//                  f = new Fixation();
		//              }
		//          }
		//          });
		//      }

		//read gaze data from eye tracker stream
		//public void readGazeStream()
		//      {
		//          _gazePointDataStream.GazePoint((x, y, timestamp) =>
		//          {
		//		sendGazeToLSL(gazeDataOutlet, x, y);

		//		//session.currentTestResults.rawGazeData.Add(new DataPoint(x, y, timestamp));
		//		////Console.WriteLine("NEW GAZE POINT: \n xPos: " + x + "\n yPos: " + y + "\n timestamp " + timestamp);
		//		//if(session.currentTestResults.startTime == -1)
		//		//{
		//		//    session.currentTestResults.startTime = timestamp; 
		//		//}
		//		//if(timestamp > session.currentTestResults.endTime)
		//		//{
		//		//    session.currentTestResults.endTime = timestamp;
		//	//}

		//	});
		//      }

		//calculate and record fixation
		//private void findSaccade()
		//{
		//    //store saccade

		//    if (session.currentTestResults.fixationData.Count > 1)
		//    {
		//        Saccade s = new Saccade();
		//        Fixation f1 = ((Fixation)session.currentTestResults.fixationData[session.currentTestResults.fixationData.Count - 2]);
		//        Fixation f2 = ((Fixation)session.currentTestResults.fixationData[session.currentTestResults.fixationData.Count - 1]);
		//        s.X1 = f1.centroid.rawX;
		//        s.Y1 = f1.centroid.rawY;
		//        s.X2 = f2.centroid.rawX;
		//        s.Y2 = f2.centroid.rawY;
		//        s.hlength = s.X2 - s.X1;
		//        s.vlength = s.Y2 - s.Y1;
		//        s.size = Math.Sqrt(Math.Pow(s.hlength, 2) + Math.Pow(s.vlength, 2));
		//        //s.start = f1.endPoint.timeStamp;
		//        //s.end = f2.startPoint.timeStamp;
		//        //s.duration = f2.startPoint.timeStamp - f1.endPoint.timeStamp;
		//        session.currentTestResults.SaccadeData.Add(s);

		//        Console.WriteLine("NEW SACCADE: \n" +
		//        "size:{0}\n" +
		//        "duration:{1}\n" +
		//        "startTime:{2}\n" +
		//        "endTime:{3}\n" +
		//        "x1Pos:{4}\n" +
		//        "y1Pos:{5}\n" +
		//        "x2Pos:{6}\n" +
		//        "y2Pos:{7}\n", s.size, s.duration, s.start, s.end, s.X1, s.Y1, s.X2, s.Y2);

		//    }
		//}


		public void Dispose()
		{
			if (_host != null)
			{
				try
				{
					_host.Dispose();
				}
				catch (System.ObjectDisposedException e)
				{
					Console.WriteLine("_host was already disposed");
				}
			}
		}
	}
}
