using LSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GazeTrackingAttentionDemo.LSLInteraction
{
	class LSLStreamInteractionHost
	{
		public double offset;

		public LSLStreamInteractionHost()
		{
			MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;
			offset = _mainWindow.stopwatch.ElapsedMilliseconds - liblsl.local_clock()*1000;
		}

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
			if (stream.eegDataStream != null)
			{
				stream.eegDataStream.Abort();
			}
			if (stream.eegDataInlet != null)
			{
				stream.eegDataInlet.close_stream();
			}
		}

		public double getOffset()
		{
			return offset;
		}
	}

	//public struct Devices
	//{
	//	public Devices(bool eg, bool ef, bool eeg)
	//	{
	//		this.eyetracker_gaze = eg;
	//		this.eyetracker_fixations = ef;
	//		this.eeg = eeg;

	//	}

	//	bool eyetracker_gaze;
	//	bool eyetracker_fixations;
	//	bool eeg;
	//}
}
