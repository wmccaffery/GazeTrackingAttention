using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeTrackingAttentionDemo.LSLInteraction
{
	class LSLStreamInteractionHost
	{

		public LSLGazeDataStream CreateNewLslGazeDataStream()
		{
			return new LSLGazeDataStream();
		}

		public LSLFixationDataStream CreateNewLslFixationDataStream()
		{
			return new LSLFixationDataStream();
		}

		public LSLEEGDataStream CreateNewLslEEGDataStream()
		{
			return new LSLEEGDataStream();
		}

		public void closeStream(LSLGazeDataStream stream)
		{
			stream.gazeStream.Abort();
			stream.gazeDataInlet.close_stream();

		}
		public void closeStream(LSLFixationDataStream stream)
		{
			//close threads
			stream.fixationBeginStream.Abort();
			stream.fixationDataStream.Abort();
			stream.fixationEndStream.Abort();

			//close lsl streams
			stream.fixationBeginInlet.close_stream();
			stream.fixationDataInlet.close_stream();
			stream.fixationEndInlet.close_stream();
		}
		public void closeStream(LSLEEGDataStream stream)
		{
			stream.eegDataStream.Abort();
			stream.eegDataInlet.close_stream();
		}
	}
}
