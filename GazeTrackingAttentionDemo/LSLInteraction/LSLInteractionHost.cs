using LSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GazeTrackingAttentionDemo.LSLInteraction
{
	class LSLInteractionHost
	{
		public double offset;
		private MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;
		
		public List<LSLDevice> Devices { get; set; }


		public LSLInteractionHost()
		{
			setLocalClockOffset();
			Devices = new List<LSLDevice>();
		}

		private void setLocalClockOffset()
		{
			offset = _mainWindow.stopwatch.ElapsedMilliseconds - liblsl.local_clock() * 1000;
		}

		//public LSLGazeDataStream CreateNewLslGazeDataStream()
		//{
		//	return new LSLGazeDataStream();
		//}

		//public LSLFixationDataStream CreateNewLslFixationDataStream()
		//{
		//	return new LSLFixationDataStream();
		//}

		//public LSLEEGDataStream CreateNewLslEEGDataStream()
		//{
		//	return new LSLEEGDataStream();
		//}

	//	public void closeStream(LSLGazeDataStream stream)
	//	{
	//		if (stream != null)
	//		{
	//			stream.gazeStream.Join();
	//			stream.gazeDataInlet.close_stream();
	//		}
	//	}
	//	public void closeStream(LSLFixationDataStream stream)
	//	{
	//		//close threads
	//		if (stream != null)
	//		{
	//			stream.fixationBeginStream.Join();
	//			stream.fixationDataStream.Join();
	//			stream.fixationEndStream.Join();

	//			//close lsl streams
	//			stream.fixationBeginInlet.close_stream();
	//			stream.fixationDataInlet.close_stream();
	//			stream.fixationEndInlet.close_stream();
	//		}
	//	}
	//	public void closeStream(LSLEEGDataStream stream)
	//	{
	//		if (stream.eegDataStream != null)
	//		{
	//			stream.eegDataStream.Join();
	//		}
	//		if (stream.eegDataInlet != null)
	//		{
	//			stream.eegDataInlet.close_stream();
	//		}
	//	}

	//	public double getOffset()
	//	{
	//		return offset;
	//	}
	//}

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
