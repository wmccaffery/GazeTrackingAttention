using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LSL;

namespace GazeTrackingAttentionDemo.LSLInteraction
{
	class LSLGazeDataStream
	{
		public liblsl.StreamInfo[] gazeDataResultsInfo;
		public liblsl.StreamInlet gazeDataInlet;
		public Thread gazeStream;
		public Boolean eyeTrackerPresent;

		public LSLGazeDataStream()
		{
			//gazeDataResultsInfo = liblsl.resolve_stream("name", "GazeData",1,1);

			//if (gazeDataResultsInfo.Length < 1)
			//{
			//	Console.WriteLine("WARNING: NO EYE TRACKER PRESENT");
			//	eyeTrackerPresent = false;
			//} else
			//{
			//	eyeTrackerPresent = true;
			//	gazeDataInlet = new liblsl.StreamInlet(gazeDataResultsInfo[0]);
			//}

		}


		//attempt to resolve stream on request
		public bool tryResolveStreams()
		{
			gazeDataResultsInfo = liblsl.resolve_stream("name", "GazeData", 1, 1);
			bool resolved;
			if (!(resolved = gazeDataResultsInfo.Length < 1))
			{
				gazeDataInlet = new liblsl.StreamInlet(gazeDataResultsInfo[0]);
			}
			return eyeTrackerPresent = !resolved;

		}

		public void GazeData(Action<double, double, double, double> action)
		{
			gazeStream = new Thread(() => getDataFromLSL(action));
			gazeStream.Start();
		}

		private void getDataFromLSL(Action<double, double, double, double> action)
		{
			while (true)
			{
				float[] sample = new float[2];
				double timestamp;
				double correction;
				timestamp = gazeDataInlet.pull_sample(sample);
				correction = gazeDataInlet.time_correction();
				action(sample[0], sample[1], sample[2], timestamp + correction);
			}
		}
	}
}
