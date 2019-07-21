using GazeTrackingAttentionDemo.DeviceInteraction;
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

namespace GazeTrackingAttentionDemo.DeviceInteraction
{
	public class DeviceInteractionHost : IDisposable
	{
		//read directly from device
		private readonly Host _host;
		private readonly FixationDataStream _fixationDataStream;
		private readonly GazePointDataStream _gazePointDataStream;


		//read from LSL
		//private readonly LSLStreamInteractionHost _lslHost;
		//private readonly LSLFixationDataStream _lslFixationDataStream;
		//private readonly LSLGazeDataStream _lslGazeDataStream;
		//private readonly LSLEEGDataStream _lslEEGDataStream;


		private MainWindow _mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;

		public List<Tuple<string, DataPoint>> rawFixationBegPoints = new List<Tuple<string, DataPoint>>();
		public List<Tuple<string, DataPoint>> rawFixationDatPoints = new List<Tuple<string, DataPoint>>();
		public List<Tuple<string, DataPoint>> rawFixationEndPoints = new List<Tuple<string, DataPoint>>();


		//LSL output streams
		public liblsl.StreamInfo gazeDataInfo;
		public liblsl.StreamOutlet gazeDataOutlet;

		public liblsl.StreamInfo fixationBeginInfo;
		public liblsl.StreamOutlet fixationBeginOutlet;

		public liblsl.StreamInfo fixationDataInfo;
		public liblsl.StreamOutlet fixationDataOutlet;

		public liblsl.StreamInfo fixationEndInfo;
		public liblsl.StreamOutlet fixationEndOutlet;

		public liblsl.StreamInfo debugDataInfo;
		public liblsl.StreamOutlet debugDataOutlet;

		public liblsl.StreamInfo debug2DataInfo;
		public liblsl.StreamOutlet debug2DataOutlet;

		double lsloffset;

		//devices
		List<LSLDevice> _lslDevices;

		//threads 
		List<Task> _tasks;

		//thread control
		CancellationTokenSource _source;
		CancellationToken _token;

		ManualResetEvent _mre;
		

		private static ManualResetEvent mre = new ManualResetEvent(false);

		//these really should be changed
		private volatile Boolean _recording;
		private volatile Boolean _streaming;

		//filepaths
		string metaData;
		string fixationCleanPath;




		public DeviceInteractionHost()
		{
			MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;
			//Device
			_host = new Host();

			//init tobii streams
			_fixationDataStream = _host.Streams.CreateFixationDataStream();
			//_fixationDataStream = _host.Streams.CreateFixationDataStream(Tobii.Interaction.Framework.FixationDataMode.Slow);
			_gazePointDataStream = _host.Streams.CreateGazePointDataStream();

			lsloffset = _mainWindow.stopwatch.ElapsedMilliseconds - liblsl.local_clock() * 1000;

			_lslDevices = new List<LSLDevice>();


			_tasks = new List<Task>();

			_source = new CancellationTokenSource();
			_token = _source.Token;

			_mre = new ManualResetEvent(false);
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

			//DEBUG
			debugDataInfo = new liblsl.StreamInfo("Debug", "Debug", 1, 500, liblsl.channel_format_t.cf_double64, "debug");
			debugDataOutlet = new liblsl.StreamOutlet(debugDataInfo);

			debug2DataInfo = new liblsl.StreamInfo("Debug2", "Debug2", 1, 500, liblsl.channel_format_t.cf_double64, "debug2");
			debug2DataOutlet = new liblsl.StreamOutlet(debug2DataInfo);

			Console.WriteLine("LSL providers intialized");
		}

		//send data to LSL
		public void sendGazeToLSL(liblsl.StreamOutlet outlet, double x, double y, double ts)
		{
			double[] data = { x, y, ts };
			outlet.push_sample(data);
		}

		//read from gaze streams in parallel
		public void feedStreamsToLSL()
		{
			Console.WriteLine("Init threads for feeding streams");

			Thread gazeBeginStream = new Thread(() => streamGazeData());
			Thread fixationBeginStream = new Thread(() => streamFixationData());

			Console.WriteLine("Starting threads for feeding streams");

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
			_mre.WaitOne();

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

			Console.WriteLine("HERE");
		}

		//DEBUG
		public void feedDebugData()
		{
			Thread debugBeginStream = new Thread(() => streamDebugData());
			debugBeginStream.Start();
		}

		public void streamDebugData()
		{
			int dat = 0;
			while (true)
			{
				_mre.WaitOne();


				float[] data = new float[1];
				data[0] = dat;
				debugDataOutlet.push_sample(data);
				dat++;

				if (_token.IsCancellationRequested)
				{
					Console.WriteLine("Debug data provider cancelled");
					break;
				}
				Thread.Sleep(2);
			}
		}

		public void feedDebug2Data()
		{
			Thread debug2BeginStream = new Thread(() => streamDebug2Data());
			debug2BeginStream.Start();
		}

		public void streamDebug2Data()
		{
			int dat = 0;
			while (true)
			{
				_mre.WaitOne();


				float[] data = new float[1];
				data[0] = dat;
				debug2DataOutlet.push_sample(data);
				dat++;

				if (_token.IsCancellationRequested)
				{
					Console.WriteLine("Debug data provider cancelled");
					break;
				}
				Thread.Sleep(2);
			}
		}

		//DEBUG

		//calibrate eye tracker
		public void calibrate()
		{
			_host.Context.LaunchConfigurationTool(Tobii.Interaction.Framework.ConfigurationTool.Recalibrate, (data) => { });

			Console.WriteLine("Calibrating eye tracker");
		}

		public void resolveStreams()
		{
			foreach(LSLDevice d in _lslDevices)
			{
				d.resolveStreams();
			}
		}

		public void RegisterLSLDevice(LSLDevice d)
		{
			_lslDevices.Add(d);
		}

		public void RegisterLocalDevice(LSLDevice d)
		{
			_lslDevices.Add(d);
		}

		//set recording directories
		public void setRecordingPaths(bool customFile, string customFilePath = "", int baselinecount = 0)
		{
			//current user
			User user = _mainWindow.currentUser;
			Test test = user.CurrentTest;


			DateTime time = DateTime.Now;
			Int32 unixts = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

			//create file metadata
			if (customFile)
			{
				String userDir = Path.GetFileName(user.DirPath);
				metaData = customFilePath + userDir + "_U" + unixts + "_baseline" + baselinecount;
			}
			else
			{
				string testDir = test.currentRecording.dataDir;
				String dirName = Path.GetFileName(testDir);
				metaData = testDir + "//" + dirName + "_U" + unixts;
			}

			//create test paths for all streams for each registered device
			foreach (LSLDevice d in _lslDevices)
			{
				foreach (LSLStream s in d.Streams)
				{
					s.FilePath = metaData + "_" + s.datasource + "_raw" + s.filename + ".csv";
				}
			}

			fixationCleanPath = metaData + "_EYETRACKER_cleanFixationData.csv";

		}

		public string getCleanFixationPath()
		{
			return fixationCleanPath;
		}

		public void createThreads()
		{
			Console.WriteLine("Creating tasks");
			foreach (LSLDevice d in _lslDevices)
			{
				if(d.Streams.Length == 0)
				{
					Console.WriteLine("No streamsfound for device " + d.Type + ". No tasks were created");
				}
				foreach(LSLStream s in d.Streams)
				{
					Console.WriteLine("task created for stream " + s.StreamInfo.name() + " from device " + s.StreamInfo.type());
					_tasks.Add(Task.Run(() => stream(s), _token));
				}
			}

			Console.Write("Done");
		}

		private void stream(LSLStream s)
		{
			while (true) {

				_mre.WaitOne();

				//t.ThrowIfCancellationRequested();

				Double[] data = s.pullData();

				double timestamp = data[data.Length - 1];

				if (!s.customts)
				{
					timestamp = timestamp * 1000;
					timestamp += lsloffset;
				}

				double[] dat = new double[data.Length - 1];
				Console.Write(s.StreamInfo.type() + " " + s.StreamInfo.name() + " Timestamp:" + timestamp);
				for(int i = 0; i < data.Length-1; i++)
				{
					Console.Write("," + data[i]);
					dat[i] = data[i];
					
				}
				Console.Write(Environment.NewLine);

				Console.WriteLine("STREAMINFOTYPE " + s.StreamInfo.type());

				if (_recording)
				{
					writeStreamToFile(s.FilePath,s.streamtype,s.datatype,s.header, dat, timestamp);
				}

				if (_token.IsCancellationRequested)
				{
					Console.WriteLine(s.StreamInfo.name() + " stream cancelled");
					break;
				}
			}
		}
		
		public void startStreaming()
		{
			//Console.WriteLine()

			//foreach (var task in _tasks)
			//{
			//	task.Start();
			//}

			_mre.Set();
		}

		public void exitThreads()
		{
			_mre.Set();
			_source.Cancel();

		}

		public void stopStreaming()
		{
			_mre.Reset();
			//int cancelledCounter = 0;
			//try
			//{
			//	_source.Cancel();

			//}
			//catch (AggregateException ae)
			//{
			//	foreach (Exception e in ae.InnerExceptions)
			//	{
			//		if (e is OperationCanceledException)
			//		{
			//			Console.WriteLine("Streaming stopped for thread " + e.Source.ToString() + ": " + ((TaskCanceledException)e).Message);
			//			cancelledCounter++;
			//			if (checkComplete(cancelledCounter))
			//			{
			//				Console.WriteLine("All tasks stopped");
			//			}
			//		}
			//		else
			//		{
			//			Console.WriteLine("Exception: " + e.GetType().Name);
			//		}
			//	}
			//}
			//finally
			//{
			//	//terminate lsl providers
			//	//_fixationDataStream.IsEnabled = false;
			//	//_gazePointDataStream.IsEnabled = false;
			//}
		}

		private bool checkComplete(int counter)
		{
			int threadcount = _tasks.Count;
			return counter == threadcount;

		}

		public void startRecording()
		{
			_recording = true;
		}

		public void stopRecording()
		{
			_recording = false;
		}
	

		public void writeStreamToFile(String rawPath, String streamType, String dataType, String fileHeader, double[] data, double timestamp)
		{
				if (!File.Exists(rawPath))
				{
					File.WriteAllText(rawPath, "");
					File.WriteAllText(rawPath, fileHeader);
				}

			double uts = (TimeSpan.FromMilliseconds(timestamp).Seconds + _mainWindow.unixStartTime);
			string datastr = "";

			if (streamType == "Fixation")
			{
				//append data into a single csv line
				//of the form "Begin," + x + "," + y + "," + timestamp + Environment.NewLine;

				datastr += dataType + ",";
				for (int i = 0; i < data.Length; i++)
				{
					datastr += (data[i] + ",");
				}
				datastr += (timestamp + "," + uts + Environment.NewLine);

				//store in program for cleaning and assigning
				switch (dataType)
				{
					case "Begin":
						rawFixationBegPoints.Add(Tuple.Create(dataType, new DataPoint((float)data[0], (float)data[1], (float)timestamp)));
						break;
					case "Data":
						rawFixationDatPoints.Add(Tuple.Create(dataType, new DataPoint((float)data[0], (float)data[1], (float)timestamp)));
						break;
					case "End":
						rawFixationEndPoints.Add(Tuple.Create(dataType, new DataPoint((float)data[0], (float)data[1], (float)timestamp)));
						break;
				}
			}
			else
			{
				for (int i = 0; i < data.Length; i++)
				{
					datastr += (data[i] + ",");
				}
				datastr += (timestamp + "," + uts + Environment.NewLine);
			}

			//write csv line to file
			File.AppendAllText(rawPath, datastr);
		}


		public void writeCleanFixationsToFile(String rawPath, Fixation fixation)
		{
			if (!File.Exists(rawPath))
				{
					File.WriteAllText(rawPath, "");
					File.WriteAllText(rawPath, "Stream,X,Y,LSLTimestamp,AdjustedUnix" + Environment.NewLine);
				}

				File.AppendAllText(rawPath, "Begin," + fixation.startPos.x + "," + fixation.startPos.y + "," + fixation.startPos.timestamp + "," + (TimeSpan.FromMilliseconds(fixation.startPos.timestamp).Seconds + _mainWindow.unixStartTime) + Environment.NewLine);
				foreach(DataPoint d in fixation.dataPos)
				{
					File.AppendAllText(rawPath, "Data," + d.x + "," + d.y + "," + d.timestamp + "," + (TimeSpan.FromMilliseconds(d.timestamp).Seconds + _mainWindow.unixStartTime) +  Environment.NewLine);
				}
				File.AppendAllText(rawPath, "End," + fixation.endPos.x + "," + fixation.endPos.y + "," + fixation.endPos.timestamp + "," + (TimeSpan.FromMilliseconds(fixation.endPos.timestamp).Seconds + _mainWindow.unixStartTime) + Environment.NewLine);
		}

		public List<Fixation> getCleanFixations()
		{
			Boolean fixationStart = false;

			List<Fixation> validFixations = new List<Fixation>();

			//valid fixation candidate
			Fixation f = new Fixation();

			//build valid fixations
			int i = 0;

			List<Tuple<string, DataPoint>> rawFixationPoints = new List<Tuple<string, DataPoint>>();
			foreach (Tuple<string, DataPoint> bd in rawFixationBegPoints)
			{
				rawFixationPoints.Add(bd);
			}
			foreach (Tuple<string, DataPoint> dd in rawFixationDatPoints)
			{
				rawFixationPoints.Add(dd);
			}
			foreach (Tuple<string, DataPoint> ed in rawFixationEndPoints)
			{
				rawFixationPoints.Add(ed);
			}

			rawFixationPoints.Sort((Tuple<string, DataPoint> a, Tuple<string, DataPoint> b) =>
			{
				//sort files into order based on number in title
				double a_num = a.Item2.timestamp;
				double b_num = b.Item2.timestamp;
				return a_num.CompareTo(b_num);
			});


			foreach (Tuple<string, DataPoint> t in rawFixationPoints)
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
