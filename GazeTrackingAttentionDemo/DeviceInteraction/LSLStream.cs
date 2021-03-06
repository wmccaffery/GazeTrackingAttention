﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static LSL.liblsl;

namespace GazeTrackingAttentionDemo.DeviceInteraction
{
	public class LSLStream
	{

		private MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;

		public StreamInfo StreamInfo { get; set;}
		public string FilePath { get; set; }

		StreamInlet streamInlet;
		//StreamOutlet streamOutlet;

		public bool customts;
		public string filename;
		public string datasource;
		public string streamtype;
		public string datatype;
		public string header;

		public LSLStream(StreamInfo streamInfo, bool customts)
		{
			this.StreamInfo = streamInfo;
			this.streamInlet = new StreamInlet(streamInfo);
			//this.streamOutlet = new StreamOutlet(streamInfo);
			this.customts = customts;

			//this shouldn't be hard coded like this, but it keeps things more consistent with the old data format
			switch(streamInfo.name())
			{
				case "FixationBegin":
					filename = "FixationBeg";
					datasource = "EYETRACKER";
					datatype = "Begin";
					streamtype = "Fixation";
					header = "DataType,X,Y,DeviceTimestamp, LSLTimestamp, AdjustedUnix" + Environment.NewLine;
					break;
				case "FixationData":
					filename = "FixationDat";
					datasource = "EYETRACKER";
					datatype = "Data";
					streamtype = "Fixation";
					header = "DataType,X,Y,DeviceTimestamp, LSLTimestamp, AdjustedUnix" + Environment.NewLine;
					break;
				case "FixationEnd":
					filename = "FixationEnd";
					datasource = "EYETRACKER";
					datatype = "End";
					streamtype = "Fixation";
					header = "DataType,X,Y,DeviceTimestamp, LSLTimestamp, AdjustedUnix" + Environment.NewLine;
					break;
				case "GazeData":
					filename = "GazeData";
					datasource = "EYETRACKER";
					datatype = "Gaze";
					streamtype = "Gaze";
					header = "X,Y,DeviceTimestamp, LSLTimestamp, AdjustedUnix" + Environment.NewLine;
					break;
				case "Debug":
					filename = "DebugData";
					datasource = "DEBUG";
					datatype = "Debug";
					streamtype = "Debug";
					header = "VAL, LSLTimestamp, AdjustedUnix" + Environment.NewLine;
					break;
				case "Debug2":
					filename = "Debug2Data";
					datasource = "DEBUG2";
					datatype = "Debug";
					streamtype = "Debug";
					header = "VAL, LSLTimestamp, AdjustedUnix" + Environment.NewLine;
					break;
				case "Quality":
					filename = "EEGQualityData";
					datasource = "EEG";
					datatype = "StreamQuality";
					streamtype = "StreamQuality";
					header = "C1,C2,C3,C4,C5,C6,C7,C8,Timestamp, AdjustedUnix" + Environment.NewLine;
					break;
				default:
					filename = "EEGData";
					datasource = "EEG";
					datatype = "EEG";
					streamtype = "EEG";
					header = "C1,C2,C3,C4,C5,C6,C7,C8,Timestamp, AdjustedUnix" + Environment.NewLine;
					break;
			}
		}

		public double[] pullData()
		{
			double[] data = new double[StreamInfo.channel_count() + 1];
			double[] sample = new double[StreamInfo.channel_count()];

			double timestamp = streamInlet.pull_sample(sample);
			timestamp += streamInlet.time_correction();

			sample.CopyTo(data, 0);

			if (customts)
			{
				timestamp += _mainWindow.stopwatch.ElapsedMilliseconds;
				data[data.Length - 1] = timestamp;
			}
			else
			{
				data[data.Length - 1] = timestamp;
			}

			return data;
		}

		//public void closeOutlet()
		//{

		//}


	}
}
