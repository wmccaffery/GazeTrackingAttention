using LSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LSL.liblsl;

namespace GazeTrackingAttentionDemo.LSLInteraction
{
	class LSLStream
	{
		private StreamInfo info;
		private StreamInlet inlet;
		private StreamOutlet outlet;

		public LSLStream(StreamInfo streamInfo)
		{
			this.info = streamInfo;
			this.inlet = new StreamInlet(streamInfo);
			this.outlet = new StreamOutlet(streamInfo);
		}

		public double[] pullData()
		{
			double[] sample = new double[info.channel_count()];
			double[] data = new double[info.channel_count() + 1];
			double timestamp;
			double correction;

			timestamp = inlet.pull_sample(sample);
			correction = inlet.time_correction();

			sample.CopyTo(data, 0);
			data[data.Length - 1] = timestamp;

			return data;

		}

		public void pushData(double[] data)
		{
			outlet.push_sample(data);
		}



		//public liblsl.StreamInfo[] fixationBeginResultsInfo;
		//public liblsl.StreamInlet fixationBeginInlet;

		//public liblsl.StreamInfo[] fixationDataResultsInfo;
		//public liblsl.StreamInlet fixationDataInlet;

		//public liblsl.StreamInfo[] fixationEndResultsInfo;
		//public liblsl.StreamInlet fixationEndInlet;

		//public Thread fixationBeginStream;
		//public Thread fixationEndStream;
		//public Thread fixationDataStream;

		//public Boolean eyeTrackerPresent;

		//private MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;

		//public LSLFixationDataStream()
		//{
		//	CancellationTokenSource source = new CancellationTokenSource();
		//	CancellationToken token = source.Token;
		//}

		//check if streams are available in LSL
		//public bool tryResolveStreams()
		//{
		//	fixationBeginResultsInfo = liblsl.resolve_stream("name", "FixationBegin", 1, 1);
		//	fixationDataResultsInfo = liblsl.resolve_stream("name", "FixationData", 1, 1);
		//	fixationEndResultsInfo = liblsl.resolve_stream("name", "FixationEnd", 1, 1);
		//	bool resolved;
		//	if ((resolved = !(fixationBeginResultsInfo.Length < 1 && fixationDataResultsInfo.Length < 1 && fixationEndResultsInfo.Length < 1)))
		//	{
		//		fixationDataInlet = new liblsl.StreamInlet(fixationDataResultsInfo[0]);
		//		fixationBeginInlet = new liblsl.StreamInlet(fixationBeginResultsInfo[0]);
		//		fixationEndInlet = new liblsl.StreamInlet(fixationEndResultsInfo[0]);
		//	}
		//	return eyeTrackerPresent = resolved;
		//}

		//public void stopStreaming()
		//{

		//}

		//public void initStream()
		//{
		//	//	fixationEndStream = new Thread(() => getEndFromLSL(action));
		//	//	fixationEndStream.Start();
		//	//	return this;
		//}

		//private void pullSampleFromLSL(liblsl.StreamInlet inlet)
		//{
		//	float[] sample = new float[3];
		//	double timestamp;
		//	double correction;

		//	timestamp = fixationBeginInlet.pull_sample(sample);
		//	correction = fixationBeginInlet.time_correction();

		//}

		//private void streamData(CancellationToken ct)
		//{
		//	while (true)
		//	{
		//		ct.ThrowIfCancellationRequested();
		//		pullSampleFromLSL(fixationBeginInlet);

		//		//start writing to file if requested
		//		//if(_isRecording){
		//		//	writeToFile()
		//		//}	

		//	}
		//}
	}
}
