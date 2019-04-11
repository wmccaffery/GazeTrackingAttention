using LSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

		public LSLFixationDataStream()
		{

			fixationBeginResultsInfo = liblsl.resolve_stream("name", "FixationBegin", 1, 1);
			fixationDataResultsInfo = liblsl.resolve_stream("name", "FixationData", 1, 1);
			fixationEndResultsInfo = liblsl.resolve_stream("name", "FixationEnd", 1, 1);

			if (fixationBeginResultsInfo.Length < 1 || fixationDataResultsInfo.Length < 1 || fixationEndResultsInfo.Length < 1) {
				Console.WriteLine("WARNING: NO EYE TRACKER PRESENT");
				eyeTrackerPresent = false;
			}
			else
			{
				eyeTrackerPresent = true;
				fixationBeginInlet = new liblsl.StreamInlet(fixationBeginResultsInfo[0]);
				fixationDataInlet = new liblsl.StreamInlet(fixationDataResultsInfo[0]);
				fixationEndInlet = new liblsl.StreamInlet(fixationEndResultsInfo[0]);
			}




			//eegDataResultsInfo = liblsl.resolve_stream("type", "EEG", 1, 1);
			//if (eegDataResultsInfo.Length < 1)
			//{
			//	Console.WriteLine("WARNING: NO EEG PRESENT");
			//	eegPresent = false;
			//}
			//else
			//{
			//	eegPresent = true;
			//	eegDataInlet = new liblsl.StreamInlet(eegDataResultsInfo[0]);
			//}
		}


		public LSLFixationDataStream Begin(Action<double, double, double> action)
		{
			fixationBeginStream = new Thread(() => recordFixationBeginStream(action));
			fixationBeginStream.Start();
			return this;

		}

		public LSLFixationDataStream Data(Action<double, double, double> action)
		{
			fixationDataStream = new Thread(() => recordFixationDataStream(action));
			fixationDataStream.Start();
			return this;

		}

		public LSLFixationDataStream End(Action<double, double, double> action)
		{
			fixationEndStream = new Thread(() => recordFixationEndStream(action));
			fixationEndStream.Start();
			return this;
		}

		private void recordFixationBeginStream(Action<double, double, double> action)
		{
			while (true)
			{
				float[] sample = new float[2];
				double timestamp;
				double correction;

				timestamp = fixationBeginInlet.pull_sample(sample);
				correction = fixationBeginInlet.time_correction();

				action(sample[0], sample[1], correction + timestamp);
			}
		}
		private void recordFixationDataStream(Action<double, double, double> action)
		{
			while (true)
			{
				float[] sample = new float[2];
				double timestamp;
				double correction;

				timestamp = fixationDataInlet.pull_sample(sample);
				correction = fixationDataInlet.time_correction();

				action(sample[0], sample[1], correction + timestamp);
			}
		}
		private void recordFixationEndStream(Action<double, double, double> action)
		{
			while (true)
			{
				float[] sample = new float[2];
				double timestamp;
				double correction;

				timestamp = fixationEndInlet.pull_sample(sample);
				correction = fixationEndInlet.time_correction();

				action(sample[0], sample[1], correction + timestamp);
			}
		}
	}


}
