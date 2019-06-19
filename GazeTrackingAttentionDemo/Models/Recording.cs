using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeTrackingAttentionDemo.Models
{
	public class Recording
	{
		public int Index { get; set; }
		public List<Fixation> fixations;
		public DataVisualization.GazePlot gp;
		public List<Saccade> saccades;
		public Boolean isPaper;
		public Boolean isCalibrated;
		public String dataDir;

		List<AOI> _aois;
		public List<AOI> Aois { get => _aois; set => _aois = value; }

		public Recording(int index, bool isPaper, String dataPath)
		{
			Index = index;
			this.isPaper = isPaper;
			this.dataDir = dataPath;
			//hold completed fixations
			fixations = new List<Fixation>();
			saccades = new List<Saccade>();
			//hold areas of interest
			Aois = new List<AOI>();
			
		}


	}
}
