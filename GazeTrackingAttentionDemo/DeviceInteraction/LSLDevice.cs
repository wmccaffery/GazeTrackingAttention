using LSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LSL.liblsl;

namespace GazeTrackingAttentionDemo.LSLInteraction
{
	public class LSLDevice
	{
		public String Type { get; set; }
		bool customts;
		public LSLStream[] Streams { get; set; }

		public LSLDevice(String type, bool customts)
		{
			this.Type = type;
			this.customts = customts;
		}

		public void resolveStreams()
		{
			Console.WriteLine("Resolving Streams...");
			StreamInfo[] streaminfo = resolve_stream("type", Type, 1, 1);
			Console.WriteLine(streaminfo.Length + " Streams found for device " + Type);
			Streams = new LSLStream[streaminfo.Length];
			for(int i = 0; i<streaminfo.Length; i++)
			{
				Streams[i] = new LSLStream(streaminfo[i], customts);
			}
			Console.WriteLine("Done");
		}
	}
}
