using LSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LSL.liblsl;

namespace GazeTrackingAttentionDemo.LSLInteraction
{
	class LSLDevice
	{

		string Type {get; set;}

		List<LSLStream> streamObjects;


		public LSLDevice(String type)
		{
			Type = type;
			streamObjects = new List<LSLStream>();
		}

	
		public bool discoverDeviceStreams()
		{
			bool streamsDiscovered = false;
			Console.WriteLine("Discovering Streams...");
			StreamInfo[] streams = resolve_stream("type", Type);
			int numStreams = streams.Count();
			if((streamsDiscovered = (numStreams > 0)))
			{
				foreach (StreamInfo s in streams)
				{
					streamObjects.Add(new LSLStream(s));
				}
				Console.WriteLine("Discovered " +  numStreams + " streams");
			} else
			{
				Console.WriteLine("No streams discovered");

			}
			return streamsDiscovered;
		}


		public List<LSLStream> getDeviceStreams()
		{
			return streamObjects;
		}

		//create a new stream for a local device
		//public void addNewLocalStreamOutlet(String name, int channels)
		//{

		//}


	}
}
