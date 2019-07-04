using GazeTrackingAttentionDemo.UserControls;
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
	class LSLEEGDataStream
	{
		public liblsl.StreamInfo[] eegDataResultsInfo;
		public liblsl.StreamInlet eegDataInlet;

		public Thread eegDataStream;
		public Boolean eegPresent;

		private MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;


		public bool tryResolveStreams()
		{
			eegDataResultsInfo = liblsl.resolve_stream("type", "EEG", 1, 1);
			bool resolved;
			if(resolved = !(eegDataResultsInfo.Length < 1))
			{
				eegDataInlet = new liblsl.StreamInlet(eegDataResultsInfo[0]);
			}
			return eegPresent = resolved;
		}

		public LSLEEGDataStream EEGData(Action<double, double, double, double, double, double, double, double, double> action)
		{
			eegDataStream = new Thread(() => getDataFromLSL(action));
			eegDataStream.Start();
			return this;
		}

		private void getDataFromLSL(Action<double, double, double, double, double, double, double, double, double> action)
		{
			eegDataInlet.open_stream();
			if (_mainWindow.currentUser.CurrentTest == null || _mainWindow.currentUser.CurrentTest.dataRecorder == null)
			{
				OverviewCtrl oc = null;
				(_mainWindow.ctrlwin.controller).Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => { oc = (OverviewCtrl)_mainWindow.ctrlwin.controller.Content; }));
				while (oc.eegDataRecorder.isStreaming())
				{
					float[] sample = new float[8];
					double timestamp;
					double correction;
					timestamp = eegDataInlet.pull_sample(sample);
					correction = eegDataInlet.time_correction();
					action(sample[0], sample[1], sample[2], sample[3], sample[4], sample[5], sample[6], sample[7], timestamp + correction + _mainWindow.stopwatch.ElapsedMilliseconds);

				}
			}
			else
			{
				while (_mainWindow.currentUser.CurrentTest.dataRecorder.isStreaming())
				{
					float[] sample = new float[8];
					double timestamp;
					double correction;
					timestamp = eegDataInlet.pull_sample(sample);
					correction = eegDataInlet.time_correction();
					action(sample[0], sample[1], sample[2], sample[3], sample[4], sample[5], sample[6], sample[7], timestamp + correction + _mainWindow.stopwatch.ElapsedMilliseconds);

				}
			}
		}
	}
}

