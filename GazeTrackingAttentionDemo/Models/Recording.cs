using Newtonsoft.Json;
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
        [JsonIgnore]
        public List<Fixation> fixations;
        [JsonIgnore]
        public DataVisualization.GazePlot gp;
        [JsonIgnore]
        public List<Saccade> saccades;
        public Boolean isPaper;
		public Boolean isCalibrated;
		public String dataDir;
		public String userID;
		public String testName;
		public double videoQpcStartTime;
		public double videoQpcEndTime;

		List<AOI> _aois;
        [JsonIgnore]
		public List<AOI> Aois { get => _aois; set => _aois = value; }

		public Recording(int index, bool isPaper, string dataDir, string name, string userID)
		{
			Index = index;
			this.isPaper = isPaper;
			this.dataDir = dataDir;
			this.testName = name;
			this.userID = userID;
			//hold areas of interest
			//Aois = new List<AOI>();
			
		}
	}
}
