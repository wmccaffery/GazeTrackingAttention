using LSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GazeTrackingAttentionDemo.LSLInteraction
{
	class LSLFixationDataStream
	{
		public liblsl.StreamInfo[] fixationBeginResultsInfo;
		public liblsl.StreamInlet fixationBeginInlet;

		public liblsl.StreamInfo[] fixationDataResultsInfo;
		public liblsl.StreamInlet fixationDataInlet;

		public liblsl.StreamInfo[] fixationEndResultsInfo;
		public liblsl.StreamInlet fixationEndInlet;

		public Thread fixationBeginStream;
		public Thread fixationEndStream;
		public Thread fixationDataStream;

		public Boolean eyeTrackerPresent;

		private MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;


		public LSLFixationDataStream()
		{

			//fixationBeginResultsInfo = liblsl.resolve_stream("name", "FixationBegin", 1, 1);
			//fixationDataResultsInfo = liblsl.resolve_stream("name", "FixationData", 1, 1);
			//fixationEndResultsInfo = liblsl.resolve_stream("name", "FixationEnd", 1, 1);

			//if (fixationBeginResultsInfo.Length < 1 || fixationDataResultsInfo.Length < 1 || fixationEndResultsInfo.Length < 1) {
			//	Console.WriteLine("WARNING: NO EYE TRACKER PRESENT");
			//	eyeTrackerPresent = false;
			//}
			//else
			//{
			//	eyeTrackerPresent = true;
			//	fixationBeginInlet = new liblsl.StreamInlet(fixationBeginResultsInfo[0]);
			//	fixationDataInlet = new liblsl.StreamInlet(fixationDataResultsInfo[0]);
			//	fixationEndInlet = new liblsl.StreamInlet(fixationEndResultsInfo[0]);
			//}
		}

		//attempt to resolve stream on request
		public bool tryResolveStreams()
		{
			fixationBeginResultsInfo = liblsl.resolve_stream("name", "FixationBegin", 1, 1);
			fixationDataResultsInfo = liblsl.resolve_stream("name", "FixationData", 1, 1);
			fixationEndResultsInfo = liblsl.resolve_stream("name", "FixationEnd", 1, 1);
			bool resolved;
			if ((resolved = !(fixationBeginResultsInfo.Length < 1 && fixationDataResultsInfo.Length < 1 && fixationEndResultsInfo.Length < 1)))
			{
				fixationDataInlet = new liblsl.StreamInlet(fixationDataResultsInfo[0]);
				fixationBeginInlet = new liblsl.StreamInlet(fixationBeginResultsInfo[0]);
				fixationEndInlet = new liblsl.StreamInlet(fixationEndResultsInfo[0]);
			}
			return eyeTrackerPresent = resolved;
		}

		public LSLFixationDataStream Begin(Action<double, double, double, double> action)
		{
			fixationBeginStream = new Thread(() => getBeginFromLSL(action));
			fixationBeginStream.Start();
			return this;

		}

		public LSLFixationDataStream Data(Action<double, double, double, double> action)
		{
			fixationDataStream = new Thread(() => getDataFromLSL(action));
			fixationDataStream.Start();
			return this;

		}

		public LSLFixationDataStream End(Action<double, double, double, double> action)
		{
			fixationEndStream = new Thread(() => getEndFromLSL(action));
			fixationEndStream.Start();
			return this;
		}

		//private void getDataFromLSL(Action<double, double, double, double> action)
		//{
		//	while (true)
		//	{
		//		float[] sample = new float[3];
		//		double timestamp;
		//		double correction;

		//		timestamp = fixationBeginInlet.pull_sample(sample);
		//		correction = fixationBeginInlet.time_correction();

		//		action(sample[0], sample[1], sample[3], correction + timestamp);
		//	}
		//}

		
		private void getBeginFromLSL(Action<double, double, double, double> action)
		{
			while (_mainWindow.currentUser.CurrentTest.dataRecorder.isStreaming())
			{
				Console.WriteLine("getBeginFromLSL");
				float[] sample = new float[3];
				double timestamp;
				double correction;

				timestamp = fixationBeginInlet.pull_sample(sample);
				correction = fixationBeginInlet.time_correction();

				action(sample[0], sample[1], sample[2], correction + timestamp);
			}
		}
		private void getDataFromLSL(Action<double, double, double, double> action)
		{
			while (_mainWindow.currentUser.CurrentTest.dataRecorder.isStreaming())
			{
				Console.WriteLine("getDataFromLSL");
				float[] sample = new float[3];
				double timestamp;
				double correction;

				timestamp = fixationDataInlet.pull_sample(sample);
				correction = fixationDataInlet.time_correction();

				action(sample[0], sample[1], sample[2], correction + timestamp);
			}
		}
		private void getEndFromLSL(Action<double, double, double, double> action)
		{
			while (_mainWindow.currentUser.CurrentTest.dataRecorder.isStreaming())
			{
				Console.WriteLine("getEndFromLSL");

				float[] sample = new float[3];
				double timestamp;
				double correction;

				timestamp = fixationEndInlet.pull_sample(sample);
				correction = fixationEndInlet.time_correction();

				action(sample[0], sample[1], sample[2], correction + timestamp);
			}
		}
	}


}
