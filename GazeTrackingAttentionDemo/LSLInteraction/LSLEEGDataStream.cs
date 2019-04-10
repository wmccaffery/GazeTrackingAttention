using LSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GazeTrackingAttentionDemo.LSLInteraction
{
	class LSLEEGDataStream
	{
		public liblsl.StreamInfo[] eegDataResultsInfo;
		public liblsl.StreamInlet eegDataInlet;

		public Thread eegDataStream;
		public Boolean eegPresent;


		public LSLEEGDataStream()
		{
				eegDataResultsInfo = liblsl.resolve_stream("type", "EEG", 1, 1);
				if(eegDataResultsInfo.Length < 1)
				{
				Console.WriteLine("WARNING: NO EEG PRESENT");
				eegPresent = false;
				} else
				{
					eegPresent = true;
					eegDataInlet = new liblsl.StreamInlet(eegDataResultsInfo[0]);
				}

		}

		public LSLEEGDataStream EEGData(Action<double, double, double, double, double, double, double, double, double> action)
		{
			eegDataStream = new Thread(() => recordEEGStream(action));
			eegDataStream.Start();
			return this;
		}

		private void recordEEGStream(Action<double, double, double, double, double, double, double, double, double> action)
		{
			while (true)
			{
				float[] sample = new float[8];
				double timestamp;
				timestamp = eegDataInlet.pull_sample(sample);
				action(sample[0], sample[1], sample[2], sample[3], sample[4], sample[5], sample[6], sample[7], timestamp);
			}
		}
	}
}

